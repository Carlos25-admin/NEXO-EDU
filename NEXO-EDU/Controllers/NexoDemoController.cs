using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NEXO_EDU.Services;

namespace NEXO_EDU.Controllers;

// Los contenidos son ficticios; los módulos requieren una sesión válida y su rol correspondiente.
public sealed class NexoDemoController : Controller
{
    [AllowAnonymous]
    [HttpGet]
    public IActionResult Index() => View();

    [Authorize(Roles = "Alumno")]
    [HttpGet]
    public IActionResult Estudiante() => View();

    [Authorize(Roles = "Docente")]
    [HttpGet]
    public IActionResult Docente() => View();

    [Authorize(Roles = "Familia")]
    [HttpGet]
    public IActionResult Familia() => View();

    [Authorize(Roles = "Alumno")]
    [HttpGet]
    public IActionResult Mascota() => View();

    [Authorize(Roles = "Alumno")]
    [HttpGet]
    public IActionResult Diagnostico() => View(DemoCurriculum.Preguntas);

    [Authorize(Roles = "Alumno")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Evaluar()
    {
        var respuestas = new Dictionary<int, int>();
        foreach (var p in DemoCurriculum.Preguntas)
        {
            if (!int.TryParse(Request.Form["q" + p.Id].ToString(), out var opcion) ||
                opcion < 0 || opcion >= p.Opciones.Length)
            {
                ModelState.AddModelError("", "Responde todas las preguntas para continuar.");
                return View("Diagnostico", DemoCurriculum.Preguntas);
            }
            respuestas[p.Id] = opcion;
        }
        return View("Resultado", DemoCurriculum.Evaluar(respuestas));
    }
}
