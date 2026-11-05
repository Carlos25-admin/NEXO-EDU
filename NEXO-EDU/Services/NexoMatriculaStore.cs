using Npgsql;
using NEXO_EDU.Models;

namespace NEXO_EDU.Services;

// Consultas sin exposición cruzada: siempre filtra por ID del docente autenticado.
public sealed class NexoMatriculaStore(IConfiguration config)
{
    private string Connection => config.GetConnectionString("NexoEdu")
        ?? throw new InvalidOperationException("Falta ConnectionStrings:NexoEdu.");

    public async Task<bool> DocenteExisteAsync(string docenteId, CancellationToken ct)
    {
        await using var cn = new NpgsqlConnection(Connection);
        await cn.OpenAsync(ct);
        await using var cmd = new NpgsqlCommand(
            "SELECT EXISTS (SELECT 1 FROM nexo.mvp_docentes WHERE usuario_id=@u)",cn);
        cmd.Parameters.AddWithValue("u",docenteId);
        return (bool)(await cmd.ExecuteScalarAsync(ct) ?? false);
    }

    public async Task CrearDocenteAsync(string docenteId, string nombre, string nivel, string institucion, CancellationToken ct)
    {
        await using var cn = new NpgsqlConnection(Connection);
        await cn.OpenAsync(ct);
        await using var cmd = new NpgsqlCommand(
            """
            INSERT INTO nexo.mvp_docentes (usuario_id,nombre,nivel,institucion)
            VALUES (@u,@n,@l,@i)
            """,cn);
        cmd.Parameters.AddWithValue("u",docenteId);
        cmd.Parameters.AddWithValue("n",nombre.Trim());
        cmd.Parameters.AddWithValue("l",nivel);
        cmd.Parameters.AddWithValue("i",institucion.Trim());
        await cmd.ExecuteNonQueryAsync(ct);
    }

    // Permite que el docente de la demo creado por 003/IdentitySeeder use el módulo.
    // No cambia su rol ni asigna acceso a alumnos existentes.
    public async Task AsegurarPerfilDemoAsync(string docenteId, string? nombre, CancellationToken ct)
    {
        await using var cn = new NpgsqlConnection(Connection);
        await cn.OpenAsync(ct);
        await using var cmd = new NpgsqlCommand(
            """
            INSERT INTO nexo.mvp_docentes(usuario_id,nombre,nivel,institucion)
            VALUES (@u,@n,'primaria','Institución de demostración')
            ON CONFLICT (usuario_id) DO NOTHING
            """,cn);
        cmd.Parameters.AddWithValue("u",docenteId);
        cmd.Parameters.AddWithValue("n",string.IsNullOrWhiteSpace(nombre)?"Docente NEXO":nombre);
        await cmd.ExecuteNonQueryAsync(ct);
    }

    public async Task<DocenteGrupo?> CargarGrupoAsync(string docenteId, CancellationToken ct)
    {
        await using var cn = new NpgsqlConnection(Connection);
        await cn.OpenAsync(ct);
        string nombre,nivel,institucion;
        await using(var cmd = new NpgsqlCommand(
            "SELECT nombre,nivel,institucion FROM nexo.mvp_docentes WHERE usuario_id=@u",cn))
        {
            cmd.Parameters.AddWithValue("u",docenteId);
            await using var rd = await cmd.ExecuteReaderAsync(ct);
            if(!await rd.ReadAsync(ct)) return null;
            nombre=rd.GetString(0);nivel=rd.GetString(1);institucion=rd.GetString(2);
        }
        var alumnos=new List<AlumnoRegistrado>();
        await using(var cmd=new NpgsqlCommand(
            """
            SELECT a.alumno_id,a.nombre,u."Email",a.nivel,a.grado,a.grupo,
                   COALESCE(p.xp,0),COALESCE(p.actividades,0)
            FROM nexo.mvp_alumnos_docente a
            JOIN public."AspNetUsers" u ON u."Id"=a.alumno_id
            LEFT JOIN nexo.mvp_progreso p ON p.usuario_id=a.alumno_id
            WHERE a.docente_id=@u
            ORDER BY a.grado,a.grupo,a.nombre
            """,cn))
        {
            cmd.Parameters.AddWithValue("u",docenteId);
            await using var rd=await cmd.ExecuteReaderAsync(ct);
            while(await rd.ReadAsync(ct))
                alumnos.Add(new(rd.GetString(0),rd.GetString(1),rd.GetString(2),rd.GetString(3),
                    rd.GetInt16(4),rd.GetString(5),rd.GetInt32(6),rd.GetInt32(7)));
        }
        return new(nombre,institucion,nivel,alumnos);
    }

    public async Task<bool> RegistrarAlumnoAsync(string docenteId,string alumnoId,
        string nombre,int grado,string grupo,CancellationToken ct)
    {
        await using var cn=new NpgsqlConnection(Connection);
        await cn.OpenAsync(ct);
        await using var cmd=new NpgsqlCommand(
            """
            INSERT INTO nexo.mvp_alumnos_docente(alumno_id,docente_id,nombre,nivel,grado,grupo)
            SELECT @a,d.usuario_id,@n,d.nivel,@g,@grupo
            FROM nexo.mvp_docentes d WHERE d.usuario_id=@u
              AND ((d.nivel='primaria' AND @g BETWEEN 1 AND 6)
                OR (d.nivel IN ('secundaria','preparatoria') AND @g BETWEEN 1 AND 3))
            """,cn);
        cmd.Parameters.AddWithValue("a",alumnoId);
        cmd.Parameters.AddWithValue("u",docenteId);
        cmd.Parameters.AddWithValue("n",nombre.Trim());
        cmd.Parameters.AddWithValue("g",grado);
        cmd.Parameters.AddWithValue("grupo",grupo.Trim().ToUpperInvariant());
        return await cmd.ExecuteNonQueryAsync(ct)==1;
    }

    public async Task<bool> PerteneceAlDocenteAsync(string docenteId,string alumnoId,CancellationToken ct)
    {
        await using var cn=new NpgsqlConnection(Connection);
        await cn.OpenAsync(ct);
        await using var cmd=new NpgsqlCommand(
            "SELECT EXISTS(SELECT 1 FROM nexo.mvp_alumnos_docente WHERE docente_id=@u AND alumno_id=@a)",cn);
        cmd.Parameters.AddWithValue("u",docenteId);
        cmd.Parameters.AddWithValue("a",alumnoId);
        return (bool)(await cmd.ExecuteScalarAsync(ct)??false);
    }
}
