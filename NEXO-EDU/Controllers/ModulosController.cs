using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NEXO_EDU.Models;
using NEXO_EDU.Services;

namespace NEXO_EDU.Controllers;

[Authorize]
public sealed class ModulosController : Controller
{
    private string NivelAlumno => NivelDemo.Normalizar(User.FindFirst("DemoLevel")?.Value);
    private string Nombre => User.FindFirst("DemoName")?.Value ?? "Estudiante";
    private ProgresoDemo Progreso => DemoProgressService.Leer(HttpContext.Session, User.Identity?.Name);

    [Authorize(Roles = "Alumno"), HttpGet]
    public IActionResult Biblioteca()
    {
        ViewBag.Nivel = NivelAlumno;
        ViewBag.Nombre = Nombre;
        ViewBag.Progreso = Progreso;
        return View(CatalogoLecciones.PorNivel(NivelAlumno));
    }

    [Authorize(Roles = "Alumno"), HttpGet]
    public IActionResult Ruta(string clave)
    {
        var modulo = CatalogoLecciones.Buscar(NivelAlumno, clave);
        if (modulo is null) return NotFound();
        var progreso = Progreso;
        ViewBag.Progreso = progreso;
        return View(new RutaNivelViewModel(modulo,
            DemoProgressService.MaximoDesbloqueado(progreso, clave), BancoNiveles.TotalNiveles,
            modulo.Preguntas.Count + Enumerable.Range(2,4).Sum(n => BancoNiveles.Obtener(modulo,n).Count)));
    }

    [Authorize(Roles = "Alumno"), HttpGet]
    public IActionResult Resolver(string clave, int nivel = 1)
    {
        var modulo = CatalogoLecciones.Buscar(NivelAlumno, clave);
        if (modulo is null || nivel is < 1 or > 5) return NotFound();
        if (nivel > DemoProgressService.MaximoDesbloqueado(Progreso, clave))
            return RedirectToAction(nameof(Ruta), new { clave });
        return View(new ActividadNivelViewModel(modulo, nivel, BancoNiveles.Obtener(modulo,nivel)));
    }

    [Authorize(Roles = "Alumno"), HttpPost, ValidateAntiForgeryToken]
    public IActionResult Calificar(string clave, int nivel = 1)
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
        var obtenido = false;
        if (aprobado)
        {
            var resumen = new ResultadoAventuraDemo(modulo.Nivel, revision.Aciertos,
                revision.Total, revision.Experiencia,
                revision.Respuestas.Select(r => new RetroalimentacionDemo(
                    r.Numero, r.Enunciado, r.Correcta, r.Explicacion, r.Acertada)).ToArray());
            obtenido = DemoProgressService.AprobarNivel(HttpContext.Session,
                User.Identity?.Name, clave, nivel, resumen, modulo.Materia);
        }
        var nivelDisponible = DemoProgressService.MaximoDesbloqueado(Progreso, clave);
        return View("Resultado", new ResultadoNivelViewModel(revision, nivel,
            aprobado, obtenido, nivelDisponible, obtenido ? revision.Experiencia : 0));
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
