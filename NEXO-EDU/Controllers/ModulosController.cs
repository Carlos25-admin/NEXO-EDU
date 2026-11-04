using System.Security.Claims;
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

    [Authorize(Roles = "Alumno")]
    [HttpGet]
    public IActionResult Biblioteca()
    {
        ViewBag.Nivel = NivelAlumno;
        ViewBag.Nombre = Nombre;
        ViewBag.Progreso = DemoProgressService.Leer(HttpContext.Session, User.Identity?.Name);
        return View(CatalogoLecciones.PorNivel(NivelAlumno));
    }

    [Authorize(Roles = "Alumno")]
    [HttpGet]
    public IActionResult Resolver(string clave)
    {
        var leccion = CatalogoLecciones.Buscar(NivelAlumno, clave);
        if (leccion is null) return NotFound();
        return View(leccion);
    }

    [Authorize(Roles = "Alumno")]
    [HttpPost, ValidateAntiForgeryToken]
    public IActionResult Calificar(string clave)
    {
        var leccion = CatalogoLecciones.Buscar(NivelAlumno, clave);
        if (leccion is null) return NotFound();
        var respuestas = new Dictionary<int,int>();
        foreach (var pregunta in leccion.Preguntas)
        {
            var entrada = Request.Form["pregunta" + pregunta.Numero].ToString();
            if (!int.TryParse(entrada, out var seleccion) || seleccion < 0 || seleccion >= pregunta.Opciones.Length)
            {
                ModelState.AddModelError("", "Completa todas las preguntas antes de enviar.");
                return View("Resolver", leccion);
            }
            respuestas.Add(pregunta.Numero, seleccion);
        }
        var resultado = CatalogoLecciones.Revisar(leccion, respuestas);
        DemoProgressService.Registrar(HttpContext.Session, User.Identity?.Name,
            new ResultadoAventuraDemo(leccion.Nivel, resultado.Aciertos, resultado.Total, resultado.Experiencia,
                resultado.Respuestas.Select(x => new RetroalimentacionDemo(
                    x.Numero, x.Enunciado, x.Correcta, x.Explicacion, x.Acertada)).ToArray()), leccion.Materia);
        return View("Resultado", resultado);
    }

    [Authorize(Roles = "Docente")]
    [HttpGet]
    public IActionResult RecursosDocente(string? nivel)
    {
        ViewBag.Nivel = NivelDemo.Normalizar(nivel);
        return View(CatalogoLecciones.PorNivel(nivel));
    }

    [Authorize(Roles = "Familia")]
    [HttpGet]
    public IActionResult GuiaFamiliar(string? nivel)
    {
        ViewBag.Nivel = NivelDemo.Normalizar(nivel);
        return View(CatalogoLecciones.PorNivel(nivel));
    }
}
