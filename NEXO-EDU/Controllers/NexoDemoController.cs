using Microsoft.AspNetCore.Mvc;
using NEXO_EDU.Services;

namespace NEXO_EDU.Controllers;

// Vistas públicas de DEMOSTRACIÓN con datos simulados; no existen usuarios autenticados aún.
public sealed class NexoDemoController : Controller
{
    [HttpGet] public IActionResult Index() => View();
    [HttpGet] public IActionResult Estudiante() => View();
    [HttpGet] public IActionResult Docente() => View();
    [HttpGet] public IActionResult Familia() => View();
    [HttpGet] public IActionResult Mascota() => View();

    [HttpGet]
    public IActionResult Diagnostico() => View(DemoCurriculum.Preguntas);

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
        var evaluacion = DemoCurriculum.Evaluar(respuestas);
        return View("Resultado", evaluacion);
    }
}
