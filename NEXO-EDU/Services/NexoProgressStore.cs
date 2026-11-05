using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Npgsql;
using NEXO_EDU.Models;

namespace NEXO_EDU.Services;

// Persistencia protegida por ClaimTypes.NameIdentifier de ASP.NET Identity.
// En modo Demo local continúa usando Session (sin necesitar PostgreSQL).
public sealed class NexoProgressStore(IConfiguration config, IWebHostEnvironment env)
{
    private bool Demo => env.IsDevelopment() && config.GetValue<bool>("DemoMode:Enabled");
    private string Connection => config.GetConnectionString("NexoEdu")
        ?? throw new InvalidOperationException("Falta ConnectionStrings:NexoEdu.");

    private static string UserId(HttpContext ctx) =>
        ctx.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
        ?? throw new UnauthorizedAccessException("No se encontró el identificador autenticado.");

    public async Task<ProgresoDemo> LeerAsync(HttpContext ctx, CancellationToken ct = default)
    {
        if (Demo) return DemoProgressService.Leer(ctx.Session, ctx.User.Identity?.Name);
        var userId = UserId(ctx);
        await using var conn = new NpgsqlConnection(Connection);
        await conn.OpenAsync(ct);
        var data = new ProgresoDemo();
        await using (var cmd = new NpgsqlCommand(
            """
            SELECT xp,actividades,ultimo_porcentaje,ultima_materia,ultima_ruta,mensaje
            FROM nexo.mvp_progreso WHERE usuario_id=@u
            """, conn))
        {
            cmd.Parameters.AddWithValue("u", userId);
            await using var reader = await cmd.ExecuteReaderAsync(ct);
            if (await reader.ReadAsync(ct))
            {
                data.Xp = reader.GetInt32(0);
                data.Actividades = reader.GetInt32(1);
                data.UltimoPorcentaje = reader.GetInt32(2);
                data.UltimaMateria = reader.IsDBNull(3) ? null : reader.GetString(3);
                data.UltimaRuta = reader.IsDBNull(4) ? null : reader.GetString(4);
                data.Mensaje = reader.IsDBNull(5) ? null : reader.GetString(5);
            }
        }
        await using (var cmd = new NpgsqlCommand(
            """
            SELECT clave,MAX(nivel) FROM nexo.mvp_niveles
            WHERE usuario_id=@u GROUP BY clave
            """, conn))
        {
            cmd.Parameters.AddWithValue("u", userId);
            await using var reader = await cmd.ExecuteReaderAsync(ct);
            while (await reader.ReadAsync(ct))
                data.NivelesSuperados[reader.GetString(0)] = reader.GetInt32(1);
        }
        return data;
    }

    // Registra también los intentos no aprobados; el desbloqueo + XP se efectúan
    // en una misma transacción y con bloqueo del registro de progreso por usuario.
    public async Task<(bool RecompensaNueva, int MaximoDesbloqueado)> GuardarNivelAsync(
        HttpContext ctx, LeccionNexo modulo, int nivel, RevisionLeccionNexo revision,
        IReadOnlyDictionary<int,int> respuestas, CancellationToken ct = default)
    {
        if (nivel is < 1 or > 5) throw new ArgumentOutOfRangeException(nameof(nivel));
        var aprobado = revision.Porcentaje >= 65;
        if (Demo)
        {
            var current = DemoProgressService.Leer(ctx.Session, ctx.User.Identity?.Name);
            if (nivel > DemoProgressService.MaximoDesbloqueado(current, modulo.Clave))
                return (false, DemoProgressService.MaximoDesbloqueado(current, modulo.Clave));
            var nuevo = aprobado && DemoProgressService.AprobarNivel(
                ctx.Session, ctx.User.Identity?.Name, modulo.Clave, nivel, Convertir(revision), modulo.Materia);
            return (nuevo, DemoProgressService.MaximoDesbloqueado(
                DemoProgressService.Leer(ctx.Session, ctx.User.Identity?.Name), modulo.Clave));
        }

        var userId = UserId(ctx);
        await using var conn = new NpgsqlConnection(Connection);
        await conn.OpenAsync(ct);
        await using var tx = await conn.BeginTransactionAsync(ct);
        await using (var insert = new NpgsqlCommand(
            "INSERT INTO nexo.mvp_progreso(usuario_id) VALUES(@u) ON CONFLICT DO NOTHING",conn,tx))
        {
            insert.Parameters.AddWithValue("u",userId);
            await insert.ExecuteNonQueryAsync(ct);
        }

        await using(var locker = new NpgsqlCommand(
            "SELECT xp FROM nexo.mvp_progreso WHERE usuario_id=@u FOR UPDATE",conn,tx))
        {
            locker.Parameters.AddWithValue("u",userId);
            await locker.ExecuteScalarAsync(ct);
        }

        int ultimo;
        await using(var cmd = new NpgsqlCommand(
            "SELECT COALESCE(MAX(nivel),0) FROM nexo.mvp_niveles WHERE usuario_id=@u AND clave=@k",conn,tx))
        {
            cmd.Parameters.AddWithValue("u",userId);
            cmd.Parameters.AddWithValue("k",modulo.Clave);
            ultimo = Convert.ToInt32(await cmd.ExecuteScalarAsync(ct));
        }
        if(nivel > Math.Min(5,ultimo+1))
        {
            await tx.RollbackAsync(ct);
            return (false, Math.Min(5,ultimo+1));
        }

        await InsertarIntentoAsync(conn,tx,userId,modulo.Clave,modulo.Materia,nivel,
            revision.Aciertos,revision.Total,revision.Porcentaje,aprobado,respuestas,ct);

        bool nueva = aprobado && nivel == ultimo+1;
        if(nueva)
        {
            await using var cmd = new NpgsqlCommand(
                """
                INSERT INTO nexo.mvp_niveles
                   (usuario_id,clave,nivel,mejor_porcentaje,xp_ganado)
                VALUES (@u,@k,@n,@p,@xp)
                """,conn,tx);
            cmd.Parameters.AddWithValue("u",userId);
            cmd.Parameters.AddWithValue("k",modulo.Clave);
            cmd.Parameters.AddWithValue("n",nivel);
            cmd.Parameters.AddWithValue("p",revision.Porcentaje);
            cmd.Parameters.AddWithValue("xp",revision.Experiencia);
            await cmd.ExecuteNonQueryAsync(ct);
            ultimo = nivel;
        }
        await ActualizarResumenAsync(conn,tx,userId,modulo.Materia,revision.Porcentaje,
            nueva?revision.Experiencia:0,ct);
        await tx.CommitAsync(ct);
        return (nueva,Math.Min(5,ultimo+1));
    }

    public async Task<ProgresoDemo> GuardarAventuraAsync(HttpContext ctx,
        ResultadoAventuraDemo resultado, IReadOnlyDictionary<int,int> respuestas,
        CancellationToken ct=default)
    {
        if (Demo) return DemoProgressService.Registrar(ctx.Session,ctx.User.Identity?.Name,resultado);
        // La aventura clásica tiene su propio banco de seis preguntas por nivel.
        // No usar un arreglo vacío: RevisionLeccionNexo.Total se calcula desde
        // Leccion.Preguntas.Count y PostgreSQL exige total > 0.
        var actividad = ActividadesDemo.Crear(resultado.Nivel);
        if (actividad.Retos.Count == 0 || actividad.Retos.Count != resultado.Total)
            throw new InvalidOperationException("La aventura no tiene un total de preguntas válido.");

        var preguntas = actividad.Retos
            .Select(r => new PreguntaNexo(r.Numero, r.Enunciado, r.Opciones,
                r.Correcta, r.Explicacion))
            .ToArray();
        var modulo = new LeccionNexo(actividad.Nivel, "aventura-" + actividad.Nivel,
            actividad.Materia, actividad.Titulo, actividad.Descripcion, "✦", preguntas);
        var revision = new RevisionLeccionNexo(modulo,resultado.Correctas,
            resultado.Detalles.Select(d => new RespuestaNexo(d.Numero,d.Enunciado,
            d.RespuestaCorrecta,d.Explicacion,d.Correcta)).ToArray(),resultado.Xp);
        await GuardarNivelAsync(ctx,modulo,1,revision,respuestas,ct);
        return await LeerAsync(ctx,ct);
    }

    private static ResultadoAventuraDemo Convertir(RevisionLeccionNexo revision) =>
        new(revision.Leccion.Nivel,revision.Aciertos,revision.Total,revision.Experiencia,
            revision.Respuestas.Select(r=>new RetroalimentacionDemo(
                r.Numero,r.Enunciado,r.Correcta,r.Explicacion,r.Acertada)).ToArray());

    private static async Task InsertarIntentoAsync(NpgsqlConnection conn, NpgsqlTransaction tx,
        string userId,string clave,string materia,int nivel,int correctas,int total,int porcentaje,
        bool aprobado,IReadOnlyDictionary<int,int> respuestas,CancellationToken ct)
    {
        await using var cmd = new NpgsqlCommand(
            """
            INSERT INTO nexo.mvp_intentos
               (id,usuario_id,clave,materia,nivel,correctas,total,porcentaje,aprobado,respuestas)
            VALUES(@id,@u,@k,@m,@n,@c,@t,@p,@a,CAST(@r AS jsonb))
            """,conn,tx);
        cmd.Parameters.AddWithValue("id",Guid.NewGuid());
        cmd.Parameters.AddWithValue("u",userId);
        cmd.Parameters.AddWithValue("k",clave);
        cmd.Parameters.AddWithValue("m",materia);
        cmd.Parameters.AddWithValue("n",nivel);
        cmd.Parameters.AddWithValue("c",correctas);
        cmd.Parameters.AddWithValue("t",total);
        cmd.Parameters.AddWithValue("p",porcentaje);
        cmd.Parameters.AddWithValue("a",aprobado);
        cmd.Parameters.AddWithValue("r",JsonSerializer.Serialize(respuestas));
        await cmd.ExecuteNonQueryAsync(ct);
    }

    private static async Task ActualizarResumenAsync(NpgsqlConnection conn,NpgsqlTransaction tx,
        string userId,string materia,int porcentaje,int xp,CancellationToken ct)
    {
        var ruta = porcentaje < 50 ? "Refuerzo guiado" :
            porcentaje < 80 ? "Práctica complementaria" : "Nuevo desafío";
        var mensaje = porcentaje < 50 ? "¡Buen intento! Practiquemos con otra explicación."
            : porcentaje < 80 ? "¡Vas avanzando! Sigue practicando."
            : "¡Excelente esfuerzo! Continúa tu camino.";
        await using var cmd = new NpgsqlCommand(
            """
            UPDATE nexo.mvp_progreso SET
                xp=xp+@xp, actividades=actividades+1,ultimo_porcentaje=@p,
                ultima_materia=@m,ultima_ruta=@r,mensaje=@msg,actualizado_en=now()
            WHERE usuario_id=@u
            """,conn,tx);
        cmd.Parameters.AddWithValue("xp",xp);
        cmd.Parameters.AddWithValue("p",porcentaje);
        cmd.Parameters.AddWithValue("m",materia);
        cmd.Parameters.AddWithValue("r",ruta);
        cmd.Parameters.AddWithValue("msg",mensaje);
        cmd.Parameters.AddWithValue("u",userId);
        await cmd.ExecuteNonQueryAsync(ct);
    }
}
