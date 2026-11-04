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
    private bool DemoLocal => environment.IsDevelopment()
        && string.IsNullOrWhiteSpace(config.GetConnectionString("NexoEdu"));

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
            // No hay contraseñas predeterminadas publicadas en el repositorio.
            // Solo credenciales guardadas localmente en User Secrets.
            var rol = new[] { "Alumno", "Docente", "Familia" }.FirstOrDefault(r =>
                string.Equals(config[$"DemoUsers:{r}:Email"], email, StringComparison.OrdinalIgnoreCase)
                && VerificarClave(model.Password, config[$"DemoUsers:{r}:Password"]));

            if (rol is null)
            {
                ModelState.AddModelError("", "Credenciales incorrectas o usuarios de demostración sin configurar en secretos locales.");
                return View(model);
            }

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, email.ToLowerInvariant()),
                new Claim(ClaimTypes.Name, email),
                new Claim(ClaimTypes.Role, rol),
                new Claim("DemoLocal", "true")
            };
            var identity = new ClaimsIdentity(claims, "NexoDemoLocal");
            await HttpContext.SignInAsync("NexoDemoLocal", new ClaimsPrincipal(identity),
                new AuthenticationProperties { IsPersistent = false, AllowRefresh = true });
            return RedirigirPorNombreDeRol(rol);
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
        // Solo las rutas locales permitidas pueden ser destinos de redirección.
        if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            return LocalRedirect(model.ReturnUrl);

        if (await users.IsInRoleAsync(user, "Docente")) return RedirigirPorNombreDeRol("Docente");
        if (await users.IsInRoleAsync(user, "Familia")) return RedirigirPorNombreDeRol("Familia");
        if (await users.IsInRoleAsync(user, "Alumno")) return RedirigirPorNombreDeRol("Alumno");

        await signIn.SignOutAsync();
        ModelState.AddModelError("", "Tu cuenta no tiene un perfil autorizado.");
        return View(model);
    }

    [Authorize, HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Salir()
    {
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
        if (User.IsInRole("Docente")) return RedirigirPorNombreDeRol("Docente");
        if (User.IsInRole("Familia")) return RedirigirPorNombreDeRol("Familia");
        if (User.IsInRole("Alumno")) return RedirigirPorNombreDeRol("Alumno");
        return RedirectToAction("Index", "NexoDemo");
    }

    private IActionResult RedirigirPorNombreDeRol(string rol) => rol switch
    {
        "Alumno" => RedirectToAction("Estudiante", "NexoDemo"),
        "Docente" => RedirectToAction("Docente", "NexoDemo"),
        "Familia" => RedirectToAction("Familia", "NexoDemo"),
        _ => RedirectToAction("Index", "NexoDemo")
    };
}
