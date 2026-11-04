using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NEXO_EDU.Data;

namespace NEXO_EDU.Services;

// Inicialización opt-in únicamente en Development, sobre tablas YA EXISTENTES.
// PasswordHash se produce con UserManager; NO se guarda contraseña en SQL ni GitHub.
public static class DemoIdentitySeeder
{
    private sealed record Cuenta(string Key, string Rol, string Nivel, string Nombre);

    public static async Task InitializeAsync(IServiceProvider services, IConfiguration config)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<NexoIdentityDbContext>();
        var roles = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var users = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

        if (!await db.Database.CanConnectAsync())
            throw new InvalidOperationException("No hay conexión PostgreSQL. Revisa ConnectionStrings:NexoEdu en User Secrets.");

        // Verificar el esquema SQL existente antes de escribir cualquier cuenta.
        var conn = db.Database.GetDbConnection();
        await conn.OpenAsync();
        try
        {
            await using var cmd = conn.CreateCommand();
            cmd.CommandText = """
                SELECT to_regclass('public."AspNetUsers"') IS NOT NULL
                   AND to_regclass('public."AspNetRoles"') IS NOT NULL
                   AND to_regclass('nexo.perfiles_usuario') IS NOT NULL
                   AND to_regclass('nexo.estudiantes') IS NOT NULL;
                """;
            if (await cmd.ExecuteScalarAsync() is not true)
                throw new InvalidOperationException("Faltan tablas del esquema NEXO EDU. Ejecuta primero los SQL 001 y 002 en EXO-EDU.");
        }
        finally { await conn.CloseAsync(); }

        var cuentas = new[]
        {
            new Cuenta("Primaria", "Alumno", "primaria", "Leo"),
            new Cuenta("Secundaria", "Alumno", "secundaria", "Sofía"),
            new Cuenta("Preparatoria", "Alumno", "preparatoria", "Alex"),
            new Cuenta("Docente", "Docente", "primaria", "Clara"),
            new Cuenta("Familia", "Familia", "primaria", "Lucía")
        };

        // Se exige contraseña privada para cada cuenta antes de hacer escrituras.
        foreach (var cuenta in cuentas)
        {
            var email = config[$"PostgresBootstrap:Accounts:{cuenta.Key}:Email"];
            var password = config[$"PostgresBootstrap:Accounts:{cuenta.Key}:Password"];
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                throw new InvalidOperationException($"Faltan credenciales locales PostgresBootstrap:Accounts:{cuenta.Key}.");
        }

        await using var tx = await db.Database.BeginTransactionAsync();
        await db.Database.ExecuteSqlRawAsync("""
            INSERT INTO nexo.instituciones(nombre,codigo)
            VALUES('Escuela de demostración NEXO EDU','NEXO-DEMO')
            ON CONFLICT (codigo) DO NOTHING;
            """);

        foreach (var role in new[] { "Alumno", "Docente", "Familia" })
        {
            if (!await roles.RoleExistsAsync(role))
                Verificar(await roles.CreateAsync(new IdentityRole(role)), "crear rol " + role);
        }

        foreach (var cuenta in cuentas)
        {
            var email = config[$"PostgresBootstrap:Accounts:{cuenta.Key}:Email"]!.Trim();
            var password = config[$"PostgresBootstrap:Accounts:{cuenta.Key}:Password"]!;
            var user = await users.FindByEmailAsync(email);
            if (user is null)
            {
                user = new IdentityUser
                {
                    Id = Guid.NewGuid().ToString("D"),
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true,
                    LockoutEnabled = true
                };
                Verificar(await users.CreateAsync(user, password), $"crear usuario {cuenta.Key}");
            }
            else
            {
                // Repetir el arranque nunca reemplaza la contraseña de una cuenta existente.
                var existingRoles = await users.GetRolesAsync(user);
                if (existingRoles.Count > 0 && !existingRoles.Contains(cuenta.Rol))
                    throw new InvalidOperationException($"El correo {email} ya tiene un rol incompatible.");
            }

            if (!await users.IsInRoleAsync(user, cuenta.Rol))
                Verificar(await users.AddToRoleAsync(user, cuenta.Rol), $"asignar rol {cuenta.Key}");

            var claims = await users.GetClaimsAsync(user);
            foreach (var (tipo, valor) in new[]
            {
                ("DemoLevel", cuenta.Nivel),
                ("DemoName", cuenta.Nombre)
            })
            {
                var existing = claims.FirstOrDefault(c => c.Type == tipo);
                if (existing is null)
                    Verificar(await users.AddClaimAsync(user, new Claim(tipo, valor)), $"crear claim {tipo}");
                else if (existing.Value != valor)
                    throw new InvalidOperationException($"Perfil existente incompatible para {email}: {tipo}");
            }

            // Perfil académico y vínculo con la institución ficticia.
            await db.Database.ExecuteSqlInterpolatedAsync($"""
                INSERT INTO nexo.perfiles_usuario(usuario_id,institucion_id,nombre_mostrar)
                SELECT {user.Id}, i.id, {cuenta.Nombre}
                FROM nexo.instituciones i WHERE i.codigo='NEXO-DEMO'
                ON CONFLICT (usuario_id) DO NOTHING;
                """);

            if (cuenta.Rol == "Alumno")
            {
                await db.Database.ExecuteSqlInterpolatedAsync($"""
                    INSERT INTO nexo.estudiantes(perfil_id,alias_publico)
                    SELECT p.id, {cuenta.Nombre}
                    FROM nexo.perfiles_usuario p WHERE p.usuario_id={user.Id}
                    ON CONFLICT (perfil_id) DO NOTHING;
                    """);

                await db.Database.ExecuteSqlInterpolatedAsync($"""
                    INSERT INTO nexo.mascotas_estudiante(estudiante_id,mascota_catalogo_id,apodo)
                    SELECT e.id, m.id, 'Nexi'
                    FROM nexo.estudiantes e
                    JOIN nexo.perfiles_usuario p ON p.id=e.perfil_id
                    CROSS JOIN nexo.mascotas_catalogo m
                    WHERE p.usuario_id={user.Id} AND m.clave='nexi'
                    ON CONFLICT (estudiante_id) DO NOTHING;
                    """);
            }
            else if (cuenta.Rol == "Docente")
            {
                await db.Database.ExecuteSqlInterpolatedAsync($"""
                    INSERT INTO nexo.docentes(perfil_id)
                    SELECT p.id FROM nexo.perfiles_usuario p WHERE p.usuario_id={user.Id}
                    ON CONFLICT (perfil_id) DO NOTHING;
                    """);
            }
            else
            {
                await db.Database.ExecuteSqlInterpolatedAsync($"""
                    INSERT INTO nexo.familiares(perfil_id)
                    SELECT p.id FROM nexo.perfiles_usuario p WHERE p.usuario_id={user.Id}
                    ON CONFLICT (perfil_id) DO NOTHING;
                    """);
            }
        }

        // El vínculo familiar de prueba no concede acceso a ningún alumno real.
        // Autorización familiar se configurará después con revisión de permisos.
        await tx.CommitAsync();
    }

    private static void Verificar(IdentityResult result, string accion)
    {
        if (!result.Succeeded)
            throw new InvalidOperationException($"Error al {accion}: " +
                string.Join("; ", result.Errors.Select(e => e.Description)));
    }
}
