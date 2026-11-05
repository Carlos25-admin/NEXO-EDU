using Npgsql;
using NEXO_EDU.Models;

namespace NEXO_EDU.Services;

// Acceso temporal: solo lectura de notas, intentos y XP.
// La relación se crea por búsqueda exacta ÚNICAMENTE en Development.
public sealed class NexoFamiliaStore(IConfiguration config)
{
    private string Connection => config.GetConnectionString("NexoEdu")
        ?? throw new InvalidOperationException("Falta ConnectionStrings:NexoEdu.");

    // Respuesta -1: sin coincidencias; -2: ambiguo; 1: vinculado.
    public async Task<int> VincularPorDatosAsync(string familiarId,BusquedaHijoModel datos,CancellationToken ct)
    {
        await using var cn=new NpgsqlConnection(Connection);
        await cn.OpenAsync(ct);
        await using var tx=await cn.BeginTransactionAsync(ct);
        var coincidencias=new List<string>();
        await using(var cmd=new NpgsqlCommand(
            """
            SELECT alumno_id FROM nexo.mvp_alumnos_docente
            WHERE lower(regexp_replace(btrim(nombre),'\s+',' ','g'))=
                  lower(regexp_replace(btrim(@nombre),'\s+',' ','g'))
              AND nivel=@nivel AND grado=@grado
            LIMIT 2
            """,cn,tx))
        {
            cmd.Parameters.AddWithValue("nombre",datos.Nombre.Trim());
            cmd.Parameters.AddWithValue("nivel",datos.Nivel);
            cmd.Parameters.AddWithValue("grado",datos.Grado);
            await using var rd=await cmd.ExecuteReaderAsync(ct);
            while(await rd.ReadAsync(ct)) coincidencias.Add(rd.GetString(0));
        }
        if(coincidencias.Count!=1)
        {
            await tx.RollbackAsync(ct);
            return coincidencias.Count==0?-1:-2;
        }
        await using(var cmd=new NpgsqlCommand(
            """
            INSERT INTO nexo.mvp_vinculos_familiares(familiar_id,alumno_id,metodo)
            VALUES(@f,@a,'coincidencia_desarrollo')
            ON CONFLICT(familiar_id,alumno_id) DO NOTHING
            """,cn,tx))
        {
            cmd.Parameters.AddWithValue("f",familiarId);
            cmd.Parameters.AddWithValue("a",coincidencias[0]);
            await cmd.ExecuteNonQueryAsync(ct);
        }
        await tx.CommitAsync(ct);
        return 1;
    }

    public async Task<IReadOnlyList<HijoResumen>> ListarHijosAsync(string familiarId,CancellationToken ct)
    {
        var hijos=new List<HijoResumen>();
        await using var cn=new NpgsqlConnection(Connection);
        await cn.OpenAsync(ct);
        await using var cmd=new NpgsqlCommand(
            """
            SELECT a.alumno_id,a.nombre,a.nivel,a.grado,a.grupo,
                COALESCE(p.xp,0),COALESCE(p.actividades,0),
                (SELECT count(*)::int FROM nexo.mvp_niveles n WHERE n.usuario_id=a.alumno_id),
                COALESCE(p.ultimo_porcentaje,0),
                (SELECT mn.nombre FROM nexo.mvp_mascotas_nombre mn WHERE mn.usuario_id=a.alumno_id)
            FROM nexo.mvp_vinculos_familiares v
            JOIN nexo.mvp_alumnos_docente a ON a.alumno_id=v.alumno_id
            LEFT JOIN nexo.mvp_progreso p ON p.usuario_id=a.alumno_id
            WHERE v.familiar_id=@f
            ORDER BY a.nombre,a.nivel,a.grado
            """,cn);
        cmd.Parameters.AddWithValue("f",familiarId);
        await using var rd=await cmd.ExecuteReaderAsync(ct);
        while(await rd.ReadAsync(ct))
            hijos.Add(new(rd.GetString(0),rd.GetString(1),rd.GetString(2),rd.GetInt16(3),rd.GetString(4),
                rd.GetInt32(5),rd.GetInt32(6),rd.GetInt32(7),rd.GetInt32(8),
                rd.IsDBNull(9)?null:rd.GetString(9)));
        return hijos;
    }

    public async Task<PanelHijo?> ConsultarAsync(string familiarId,string alumnoId,CancellationToken ct)
    {
        // Seguridad: siempre comprobar vínculo usando familiarId de Identity.
        var hijos=await ListarHijosAsync(familiarId,ct);
        var hijo=hijos.FirstOrDefault(h=>h.AlumnoId==alumnoId);
        if(hijo is null) return null;
        var intentos=new List<HijoIntento>();
        var materias=new List<HijoMateria>();
        await using var cn=new NpgsqlConnection(Connection);
        await cn.OpenAsync(ct);
        await using(var cmd=new NpgsqlCommand(
            """
            SELECT i.materia,i.nivel,i.porcentaje,i.aprobado,i.fecha
            FROM nexo.mvp_intentos i
            WHERE i.usuario_id=@a
              AND EXISTS(SELECT 1 FROM nexo.mvp_vinculos_familiares v
                         WHERE v.familiar_id=@f AND v.alumno_id=i.usuario_id)
            ORDER BY i.fecha DESC LIMIT 15
            """,cn))
        {
            cmd.Parameters.AddWithValue("a",alumnoId);
            cmd.Parameters.AddWithValue("f",familiarId);
            await using var rd=await cmd.ExecuteReaderAsync(ct);
            while(await rd.ReadAsync(ct))
                intentos.Add(new(rd.GetString(0),rd.GetInt32(1),rd.GetInt32(2),rd.GetBoolean(3),rd.GetDateTime(4)));
        }
        await using(var cmd=new NpgsqlCommand(
            """
            SELECT n.clave,MAX(n.nivel),MAX(n.mejor_porcentaje)
            FROM nexo.mvp_niveles n
            WHERE n.usuario_id=@a
              AND EXISTS(SELECT 1 FROM nexo.mvp_vinculos_familiares v
                         WHERE v.familiar_id=@f AND v.alumno_id=n.usuario_id)
            GROUP BY n.clave ORDER BY n.clave
            """,cn))
        {
            cmd.Parameters.AddWithValue("a",alumnoId);
            cmd.Parameters.AddWithValue("f",familiarId);
            await using var rd=await cmd.ExecuteReaderAsync(ct);
            while(await rd.ReadAsync(ct))
                materias.Add(new(rd.GetString(0),rd.GetInt32(1),rd.GetInt32(2)));
        }
        return new(hijo,intentos,materias);
    }
}
