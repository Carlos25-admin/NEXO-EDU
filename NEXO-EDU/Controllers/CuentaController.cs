using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NEXO_EDU.Models;

namespace NEXO_EDU.Controllers;

public sealed class CuentaController(SignInManager<IdentityUser> signIn, UserManager<IdentityUser> users) : Controller
{
    [AllowAnonymous]
    [HttpGet]
    public IActionResult Ingresar(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true) return RedirigirPorRolActual();
        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [AllowAnonymous]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Ingresar(LoginViewModel model)
    {
        if (!ModelState.IsValid) return View(model);
        var user = await users.FindByEmailAsync(model.Email.Trim());
        if (user is null)
        {
            ModelState.AddModelError("", "Credenciales incorrectas.");
            return View(model);
        }

        var result = await signIn.PasswordSignInAsync(user, model.Password, isPersistent: false, lockoutOnFailure: true);
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

        if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            return LocalRedirect(model.ReturnUrl);

        // En la misma solicitud User no refleja todavía la cookie recién emitida.
        // Los roles deben consultarse desde el usuario autenticado en Identity.
        if (await users.IsInRoleAsync(user, "Docente"))
            return RedirectToAction("Docente", "NexoDemo");
        if (await users.IsInRoleAsync(user, "Familia"))
            return RedirectToAction("Familia", "NexoDemo");
        if (await users.IsInRoleAsync(user, "Alumno"))
            return RedirectToAction("Estudiante", "NexoDemo");

        await signIn.SignOutAsync();
        ModelState.AddModelError("", "Tu cuenta no tiene un perfil autorizado.");
        return View(model);
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Salir()
    {
        await signIn.SignOutAsync();
        return RedirectToAction("Index", "NexoDemo");
    }

    [AllowAnonymous]
    [HttpGet]
    public IActionResult AccesoDenegado() => View();

    private IActionResult RedirigirPorRolActual()
    {
        if (User.IsInRole("Docente")) return RedirectToAction("Docente", "NexoDemo");
        if (User.IsInRole("Familia")) return RedirectToAction("Familia", "NexoDemo");
        if (User.IsInRole("Alumno")) return RedirectToAction("Estudiante", "NexoDemo");
        return RedirectToAction("Index", "NexoDemo");
    }
}
