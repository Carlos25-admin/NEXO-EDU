using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Npgsql;

namespace NEXO_EDU.Services;

// La identidad autenticada determina el dueño del nombre; nunca se recibe usuario_id del formulario.
public sealed class NexoPetNameStore(IConfiguration config, IWebHostEnvironment env)
{
    private bool Demo => env.IsDevelopment() && config.GetValue<bool>("DemoMode:Enabled");
    private string Connection => config.GetConnectionString("NexoEdu")
        ?? throw new InvalidOperationException("Falta ConnectionStrings:NexoEdu.");

    private static string StudentId(HttpContext context) =>
        context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
        ?? throw new UnauthorizedAccessException("No se encontró el alumno autenticado.");

    private static string SessionKey(string userId) =>
        "NexoPet:Nombre:" + userId.Trim().ToLowerInvariant();

    public Task<string?> LeerAsync(HttpContext context, CancellationToken ct = default)
        => LeerPorIdAsync(StudentId(context), context.Session, ct);

    // La sobrecarga se usa inmediatamente después de autenticarse, antes de que
    // HttpContext.User se reconstruya con la nueva cookie de Identity.
    public async Task<string?> LeerPorIdAsync(string userId, ISession session, CancellationToken ct = default)
    {
        if (Demo) return session.GetString(SessionKey(userId));
        await using var conn = new NpgsqlConnection(Connection);
        await conn.OpenAsync(ct);
        await using var cmd = new NpgsqlCommand(
            "SELECT nombre FROM nexo.mvp_mascotas_nombre WHERE usuario_id=@u", conn);
        cmd.Parameters.AddWithValue("u", userId);
        return (string?)await cmd.ExecuteScalarAsync(ct);
    }

    public async Task GuardarAsync(HttpContext context, string nombre, CancellationToken ct = default)
    {
        var userId = StudentId(context);
        if (Demo)
        {
            context.Session.SetString(SessionKey(userId), nombre);
            return;
        }
        await using var conn = new NpgsqlConnection(Connection);
        await conn.OpenAsync(ct);
        await using var cmd = new NpgsqlCommand(
            """
            INSERT INTO nexo.mvp_mascotas_nombre(usuario_id,nombre,actualizado_en)
            VALUES(@u,@n,now())
            ON CONFLICT(usuario_id)
            DO UPDATE SET nombre=EXCLUDED.nombre, actualizado_en=now()
            """,conn);
        cmd.Parameters.AddWithValue("u",userId);
        cmd.Parameters.AddWithValue("n",nombre);
        await cmd.ExecuteNonQueryAsync(ct);
    }
}
