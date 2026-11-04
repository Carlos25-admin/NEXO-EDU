using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NEXO_EDU.Models;
using NEXO_EDU.Services;

namespace NEXO_EDU.Controllers;

// Etapa de prototipo: perfiles protegidos por rol, niveles seleccionables solo para
// comparar diseños sin datos reales. El nivel definitivo vendrá de la matrícula.
public sealed class NexoDemoController : Controller
{
    [AllowAnonymous, HttpGet]
    public IActionResult Index() => View();

    [Authorize(Roles = "Alumno"), HttpGet]
    public IActionResult Estudiante(string? nivel)
    {
        ViewBag.Nivel = NivelDemo.Normalizar(nivel);
        return View();
    }

    [Authorize(Roles = "Docente"), HttpGet]
    public IActionResult Docente(string? nivel)
    {
        ViewBag.Nivel = NivelDemo.Normalizar(nivel);
        return View();
    }

    [Authorize(Roles = "Familia"), HttpGet]
    public IActionResult Familia(string? nivel)
    {
        ViewBag.Nivel = NivelDemo.Normalizar(nivel);
        return View();
    }

    [Authorize(Roles = "Alumno"), HttpGet]
    public IActionResult Mascota() => View();

    [Authorize(Roles = "Alumno"), HttpGet]
    public IActionResult Diagnostico() => View(DemoCurriculum.Preguntas);

    [Authorize(Roles = "Alumno"), HttpPost, ValidateAntiForgeryToken]
    public IActionResult Evaluar()
    {
        var respuestas = new Dictionary<int, int>();
        foreach (var p in DemoCurriculum.Preguntas)
        {
            if (!int.TryParse(Request.Form["q" + p.Id].ToString(), out var opcion)
                || opcion < 0 || opcion >= p.Opciones.Length)
            {
                ModelState.AddModelError("", "Responde todas las preguntas para continuar.");
                return View("Diagnostico", DemoCurriculum.Preguntas);
            }
            respuestas[p.Id] = opcion;
        }
        return View("Resultado", DemoCurriculum.Evaluar(respuestas));
    }

    [Authorize(Roles = "Alumno"), HttpGet]
    public IActionResult Aventura(string? nivel) => View(ActividadesDemo.Crear(nivel));

    [Authorize(Roles = "Alumno"), HttpPost, ValidateAntiForgeryToken]
    public IActionResult CorregirAventura(string? nivel)
    {
        var actividad = ActividadesDemo.Crear(nivel);
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
        return View("ResultadoAventura", ActividadesDemo.Evaluar(actividad.Nivel, respuestas));
    }
}
