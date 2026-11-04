using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NEXO_EDU.Data;

namespace NEXO_EDU.Services;

// Crea tres cuentas ficticias en una base PostgreSQL LOCAL completamente vacía.
// Las contraseñas llegan exclusivamente por User Secrets; nunca se guardan en GitHub.
public static class DemoIdentitySeeder
{
    public static async Task InitializeAsync(IServiceProvider services, IConfiguration config)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<NexoDbContext>();
        var roles = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var users = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

        // Solo para bootstrap del MVP: usar EF migrations para despliegues reales.
        await db.Database.EnsureCreatedAsync();

        foreach (var role in new[] { "Alumno", "Docente", "Familia" })
        {
            if (!await roles.RoleExistsAsync(role))
            {
                var result = await roles.CreateAsync(new IdentityRole(role));
                if (!result.Succeeded) throw new InvalidOperationException(
                    "No se pudo crear el rol " + role + ": " + string.Join("; ", result.Errors.Select(x => x.Description)));
            }
        }

        foreach (var (key, role) in new[] { ("Alumno", "Alumno"), ("Docente", "Docente"), ("Familia", "Familia") })
        {
            var email = config[$"DemoUsers:{key}:Email"];
            var password = config[$"DemoUsers:{key}:Password"];
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                throw new InvalidOperationException($"Falta DemoUsers:{key}:Email o Password en User Secrets.");

            var user = await users.FindByEmailAsync(email);
            if (user is null)
            {
                user = new IdentityUser { UserName = email, Email = email, EmailConfirmed = true };
                var created = await users.CreateAsync(user, password);
                if (!created.Succeeded) throw new InvalidOperationException(
                    $"Error al crear usuario de prueba {key}: " + string.Join("; ", created.Errors.Select(x => x.Description)));
            }
            if (!await users.IsInRoleAsync(user, role))
            {
                var assigned = await users.AddToRoleAsync(user, role);
                if (!assigned.Succeeded) throw new InvalidOperationException("No se pudo asignar el rol " + role);
            }
        }
    }
}
