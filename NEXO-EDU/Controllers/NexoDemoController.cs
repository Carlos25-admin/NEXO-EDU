using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NEXO_EDU.Models;
using NEXO_EDU.Services;

namespace NEXO_EDU.Controllers;

// Los alumnos de demo tienen nivel fijo por claim. Docentes y familias pueden
// explorar las tres presentaciones sin cambiar de rol ni acceder a datos reales.
public sealed class NexoDemoController : Controller
{
    private string NivelAlumno => User.FindFirst("DemoLevel")?.Value is string valor
        ? NivelDemo.Normalizar(valor) : NivelDemo.Primaria;

    private string NivelDeVista(string? solicitado) =>
        User.IsInRole("Alumno")
            ? NivelAlumno : NivelDemo.Normalizar(solicitado);

    private void PrepararAlumno()
    {
        ViewBag.Nivel = NivelAlumno;
        ViewBag.Nombre = User.FindFirst("DemoName")?.Value ?? "Estudiante";
        ViewBag.Progreso = DemoProgressService.Leer(HttpContext.Session, User.Identity?.Name);
    }

    [AllowAnonymous, HttpGet]
    public IActionResult Index() => View();

    [Authorize(Roles = "Alumno"), HttpGet]
    public IActionResult Estudiante(string? nivel)
    {
        PrepararAlumno();
        return View();
    }

    [Authorize(Roles = "Docente"), HttpGet]
    public IActionResult Docente(string? nivel)
    {
        ViewBag.Nivel = NivelDeVista(nivel);
        return View();
    }

    [Authorize(Roles = "Familia"), HttpGet]
    public IActionResult Familia(string? nivel)
    {
        ViewBag.Nivel = NivelDeVista(nivel);
        return View();
    }

    [Authorize(Roles = "Alumno"), HttpGet]
    public IActionResult Mascota()
    {
        PrepararAlumno();
        return View();
    }

    [Authorize(Roles = "Alumno"), HttpGet]
    public IActionResult Diagnostico() => RedirectToAction("Aventura", new { nivel = NivelAlumno });

    [Authorize(Roles = "Alumno"), HttpPost, ValidateAntiForgeryToken]
    public IActionResult Evaluar() => RedirectToAction("Aventura", new { nivel = NivelAlumno });

    [Authorize(Roles = "Alumno"), HttpGet]
    public IActionResult Aventura(string? nivel)
    {
        ViewBag.Progreso = DemoProgressService.Leer(HttpContext.Session, User.Identity?.Name);
        return View(ActividadesDemo.Crear(NivelDeVista(nivel)));
    }

    [Authorize(Roles = "Alumno"), HttpPost, ValidateAntiForgeryToken]
    public IActionResult CorregirAventura(string? nivel)
    {
        var actividad = ActividadesDemo.Crear(NivelDeVista(nivel));
        var respuestas = new Dictionary<int, int>();
        foreach (var reto in actividad.Retos)
        {
            if (!int.TryParse(Request.Form["reto" + reto.Numero].ToString(), out var n)
                || n < 0 || n >= reto.Opciones.Length)
            {
                ModelState.AddModelError("", "Contesta todos los retos para terminar tu aventura.");
                return View("Aventura", actividad);
            }
            respuestas[reto.Numero] = n;
        }

        var resultado = ActividadesDemo.Evaluar(actividad.Nivel, respuestas);
        ViewBag.Progreso = DemoProgressService.Registrar(HttpContext.Session, User.Identity?.Name, resultado);
        return View("ResultadoAventura", resultado);
    }
}
