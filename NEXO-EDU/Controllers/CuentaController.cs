using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NEXO_EDU.Models;

namespace NEXO_EDU.Controllers;

public sealed class CuentaController(IServiceProvider services, IWebHostEnvironment environment, IConfiguration config) : Controller
{
    private bool DemoLocal => environment.IsDevelopment() && config.GetValue<bool>("DemoMode:Enabled");

    [AllowAnonymous, HttpGet]
    public IActionResult Ingresar(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true) return RedirigirPorRolActual();
        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [AllowAnonymous, HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Ingresar(LoginViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        var email = model.Email.Trim();

        if (DemoLocal)
        {
            // Cuentas temporales, exclusivamente en Development. No utilizar en servidores ni con datos reales.
            var perfiles = new[]
            {
                new { Email = config["DemoStudents:Primaria:Email"], Password = config["DemoStudents:Primaria:Password"], Role = "Alumno", Level = "primaria", Name = config["DemoStudents:Primaria:Name"] },
                new { Email = config["DemoStudents:Secundaria:Email"], Password = config["DemoStudents:Secundaria:Password"], Role = "Alumno", Level = "secundaria", Name = config["DemoStudents:Secundaria:Name"] },
                new { Email = config["DemoStudents:Preparatoria:Email"], Password = config["DemoStudents:Preparatoria:Password"], Role = "Alumno", Level = "preparatoria", Name = config["DemoStudents:Preparatoria:Name"] },
                new { Email = config["DemoUsers:Docente:Email"], Password = config["DemoUsers:Docente:Password"], Role = "Docente", Level = "primaria", Name = "Clara" },
                new { Email = config["DemoUsers:Familia:Email"], Password = config["DemoUsers:Familia:Password"], Role = "Familia", Level = "primaria", Name = "Lucía" }
            };

            var perfil = perfiles.FirstOrDefault(p =>
                !string.IsNullOrWhiteSpace(p.Email)
                && string.Equals(p.Email, email, StringComparison.OrdinalIgnoreCase)
                && VerificarClave(model.Password, p.Password));

            if (perfil is null)
            {
                ModelState.AddModelError("", "Credenciales de demostración incorrectas.");
                return View(model);
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, email.ToLowerInvariant()),
                new Claim(ClaimTypes.Name, email),
                new Claim(ClaimTypes.Role, perfil.Role),
                new Claim("DemoLevel", perfil.Level),
                new Claim("DemoName", perfil.Name ?? "Estudiante"),
                new Claim("DemoLocal", "true")
            };
            var identity = new ClaimsIdentity(claims, "NexoDemoLocal");
            await HttpContext.SignInAsync("NexoDemoLocal", new ClaimsPrincipal(identity),
                new AuthenticationProperties { IsPersistent = false, AllowRefresh = true });
            return RedirigirPorNombreDeRol(perfil.Role, perfil.Level);
        }

        var users = services.GetRequiredService<UserManager<IdentityUser>>();
        var signIn = services.GetRequiredService<SignInManager<IdentityUser>>();
        var user = await users.FindByEmailAsync(email);
        if (user is null)
        {
            ModelState.AddModelError("", "Credenciales incorrectas.");
            return View(model);
        }
        var result = await signIn.PasswordSignInAsync(user, model.Password, false, true);
        if (result.IsLockedOut)
        {
            ModelState.AddModelError("", "Cuenta temporalmente bloqueada por intentos fallidos.");
            return View(model);
        }
        if (!result.Succeeded)
        {
            ModelState.AddModelError("", "Credenciales incorrectas.");
            return View(model);
        }

        // Se ignora ReturnUrl en login: siempre se redirige según roles y nivel persistidos.
        if (await users.IsInRoleAsync(user, "Docente"))
        {
            var claims = await users.GetClaimsAsync(user);
            var nivel = claims.FirstOrDefault(c => c.Type == "DemoLevel")?.Value ?? "primaria";
            return RedirigirPorNombreDeRol("Docente", nivel);
        }
        if (await users.IsInRoleAsync(user, "Familia")) return RedirigirPorNombreDeRol("Familia", "primaria");
        if (await users.IsInRoleAsync(user, "Alumno"))
        {
            var claims = await users.GetClaimsAsync(user);
            var nivel = claims.FirstOrDefault(c => c.Type == "DemoLevel")?.Value ?? "primaria";
            return RedirigirPorNombreDeRol("Alumno", nivel);
        }

        await signIn.SignOutAsync();
        ModelState.AddModelError("", "Tu cuenta no tiene un perfil autorizado.");
        return View(model);
    }

    [Authorize, HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Salir()
    {
        HttpContext.Session.Clear();
        if (DemoLocal) await HttpContext.SignOutAsync("NexoDemoLocal");
        else await services.GetRequiredService<SignInManager<IdentityUser>>().SignOutAsync();
        return RedirectToAction("Index", "NexoDemo");
    }

    [AllowAnonymous, HttpGet]
    public IActionResult AccesoDenegado() => View();

    private static bool VerificarClave(string recibida, string? esperada)
    {
        if (string.IsNullOrEmpty(esperada) || string.IsNullOrEmpty(recibida)) return false;
        var a = SHA256.HashData(Encoding.UTF8.GetBytes(recibida));
        var b = SHA256.HashData(Encoding.UTF8.GetBytes(esperada));
        return CryptographicOperations.FixedTimeEquals(a, b);
    }

    private IActionResult RedirigirPorRolActual()
    {
        if (User.IsInRole("Docente")) return RedirigirPorNombreDeRol("Docente", User.FindFirst("DemoLevel")?.Value ?? "primaria");
        if (User.IsInRole("Familia")) return RedirigirPorNombreDeRol("Familia", "primaria");
        if (User.IsInRole("Alumno")) return RedirigirPorNombreDeRol("Alumno", User.FindFirst("DemoLevel")?.Value ?? "primaria");
        return RedirectToAction("Index", "NexoDemo");
    }

    private IActionResult RedirigirPorNombreDeRol(string rol, string nivel) => rol switch
    {
        "Alumno" => RedirectToAction("Estudiante", "NexoDemo", new { nivel }),
        "Docente" => RedirectToAction("Docente", "NexoDemo", new { nivel }),
        "Familia" => RedirectToAction("Familia", "NexoDemo", new { nivel }),
        _ => RedirectToAction("Index", "NexoDemo")
    };
}
