using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NEXO_EDU.Models;
using NEXO_EDU.Services;

namespace NEXO_EDU.Controllers;

[Authorize]
public sealed class ModulosController(NexoProgressStore store) : Controller
{
    private string NivelAlumno => NivelDemo.Normalizar(User.FindFirst("DemoLevel")?.Value);
    private string Nombre => User.FindFirst("DemoName")?.Value ?? "Estudiante";
    private Task<ProgresoDemo> ProgresoAsync() => store.LeerAsync(HttpContext, HttpContext.RequestAborted);

    [Authorize(Roles = "Alumno"), HttpGet]
    public async Task<IActionResult> Biblioteca()
    {
        ViewBag.Nivel = NivelAlumno;
        ViewBag.Nombre = Nombre;
        ViewBag.Progreso = await ProgresoAsync();
        return View(CatalogoLecciones.PorNivel(NivelAlumno));
    }

    [Authorize(Roles = "Alumno"), HttpGet]
    public async Task<IActionResult> Ruta(string clave)
    {
        var modulo = CatalogoLecciones.Buscar(NivelAlumno, clave);
        if (modulo is null) return NotFound();
        var progreso = await ProgresoAsync();
        ViewBag.Progreso = progreso;
        return View(new RutaNivelViewModel(modulo,
            DemoProgressService.MaximoDesbloqueado(progreso, clave), BancoNiveles.TotalNiveles,
            modulo.Preguntas.Count + Enumerable.Range(2,4).Sum(n => BancoNiveles.Obtener(modulo,n).Count)));
    }

    [Authorize(Roles = "Alumno"), HttpGet]
    public async Task<IActionResult> Resolver(string clave, int nivel = 1)
    {
        var modulo = CatalogoLecciones.Buscar(NivelAlumno, clave);
        if (modulo is null || nivel is < 1 or > 5) return NotFound();
        if (nivel > DemoProgressService.MaximoDesbloqueado(await ProgresoAsync(), clave))
            return RedirectToAction(nameof(Ruta), new { clave });
        return View(new ActividadNivelViewModel(modulo, nivel, BancoNiveles.Obtener(modulo,nivel)));
    }

    [Authorize(Roles = "Alumno"), HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Calificar(string clave, int nivel = 1)
    {
        var modulo = CatalogoLecciones.Buscar(NivelAlumno, clave);
        if (modulo is null || nivel is < 1 or > 5) return NotFound();
        if (nivel > DemoProgressService.MaximoDesbloqueado(Progreso, clave))
            return RedirectToAction(nameof(Ruta), new { clave });

        var preguntas = BancoNiveles.Obtener(modulo,nivel);
        var respuestas = new Dictionary<int,int>();
        foreach (var pregunta in preguntas)
        {
            var entrada = Request.Form["pregunta" + pregunta.Numero].ToString();
            if (!int.TryParse(entrada, out var seleccion) ||
                seleccion < 0 || seleccion >= pregunta.Opciones.Length)
            {
                ModelState.AddModelError("", "Responde todas las preguntas antes de continuar.");
                return View("Resolver", new ActividadNivelViewModel(modulo, nivel, preguntas));
            }
            respuestas[pregunta.Numero] = seleccion;
        }
        // El banco del nivel solicitado es la única fuente de respuestas correctas.
        var actividad = modulo with { Preguntas = preguntas };
        var revision = CatalogoLecciones.Revisar(actividad, respuestas);
        var aprobado = revision.Porcentaje >= 65;
        var (nueva, desbloqueado) = await store.GuardarNivelAsync(
            HttpContext, modulo, nivel, revision, respuestas, HttpContext.RequestAborted);
        return View("Resultado", new ResultadoNivelViewModel(revision, nivel,
            aprobado, nueva, desbloqueado, nueva ? revision.Experiencia : 0));
    }

    [Authorize(Roles = "Docente"), HttpGet]
    public IActionResult RecursosDocente(string? nivel)
    {
        ViewBag.Nivel = NivelDemo.Normalizar(nivel);
        return View(CatalogoLecciones.PorNivel(nivel));
    }

    [Authorize(Roles = "Familia"), HttpGet]
    public IActionResult GuiaFamiliar(string? nivel)
    {
        ViewBag.Nivel = NivelDemo.Normalizar(nivel);
        return View(CatalogoLecciones.PorNivel(nivel));
    }
}
