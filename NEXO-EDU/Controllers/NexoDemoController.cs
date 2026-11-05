using Microsoft.AspNetCore.Authorization;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using NEXO_EDU.Models;
using NEXO_EDU.Services;

namespace NEXO_EDU.Controllers;

// Los alumnos de demo tienen nivel fijo por claim. Docentes y familias pueden
// explorar las tres presentaciones sin cambiar de rol ni acceder a datos reales.
public sealed class NexoDemoController(NexoProgressStore store, NexoMatriculaStore matriculas, NexoPetNameStore petNames, IConfiguration config, IWebHostEnvironment env) : Controller
{
    private string NivelAlumno => User.FindFirst("DemoLevel")?.Value is string valor
        ? NivelDemo.Normalizar(valor) : NivelDemo.Primaria;

    private string NivelDeVista(string? solicitado) =>
        (User.IsInRole("Alumno") || User.IsInRole("Docente"))
            ? NivelAlumno : NivelDemo.Normalizar(solicitado);

    private async Task PrepararAlumnoAsync()
    {
        ViewBag.Nivel = NivelAlumno;
        ViewBag.Nombre = User.FindFirst("DemoName")?.Value ?? "Estudiante";
        ViewBag.Progreso = await store.LeerAsync(HttpContext, HttpContext.RequestAborted);
        ViewBag.NombreMascota = await petNames.LeerAsync(HttpContext, HttpContext.RequestAborted);
    }

    [AllowAnonymous, HttpGet]
    public IActionResult Index() => View();

    [Authorize(Roles = "Alumno"), HttpGet]
    public async Task<IActionResult> Estudiante(string? nivel)
    {
        await PrepararAlumnoAsync();
        return View();
    }

    [Authorize(Roles = "Docente"), HttpGet]
    public async Task<IActionResult> Docente(string? nivel)
    {
        ViewBag.Nivel = NivelDeVista(nivel);
        ViewBag.Nombre = User.FindFirst("DemoName")?.Value ?? "Docente";
        if (!(env.IsDevelopment() && config.GetValue<bool>("DemoMode:Enabled")))
        {
            var id=User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if(!string.IsNullOrWhiteSpace(id))
                ViewBag.Grupo = await matriculas.CargarGrupoAsync(id,HttpContext.RequestAborted);
        }
        return View();
    }

    [Authorize(Roles = "Familia"), HttpGet]
    public IActionResult Familia(string? nivel)
    {
        ViewBag.Nivel = NivelDeVista(nivel);
        return View();
    }

    [Authorize(Roles = "Alumno"), HttpGet]
    public async Task<IActionResult> Mascota()
    {
        await PrepararAlumnoAsync();
        return View();
    }

    [Authorize(Roles = "Alumno"), HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> NombrarMascota(string? nombre)
    {
        var elegido = Regex.Replace((nombre ?? "").Trim(), @"\s+", " ");
        // Nombres cortos legibles en español; sin caracteres de marcado ni control.
        if (elegido.Length is < 2 or > 24 ||
            !Regex.IsMatch(elegido, @"^[\p{L}\p{M}0-9 ]+$"))
        {
            TempData["PetNameError"] = "Elige un nombre de 2 a 24 caracteres usando letras, números o espacios.";
            return RedirectToAction(nameof(Mascota));
        }

        await petNames.GuardarAsync(HttpContext, elegido, HttpContext.RequestAborted);
        TempData["PetNameSaved"] = "¡Ahora tu mascota se llama " + elegido + "!";
        return RedirectToAction(nameof(Mascota));
    }

    [Authorize(Roles = "Alumno"), HttpGet]
    public IActionResult Diagnostico() => RedirectToAction("Aventura", new { nivel = NivelAlumno });

    [Authorize(Roles = "Alumno"), HttpPost, ValidateAntiForgeryToken]
    public IActionResult Evaluar() => RedirectToAction("Aventura", new { nivel = NivelAlumno });

    [Authorize(Roles = "Alumno"), HttpGet]
    public async Task<IActionResult> Aventura(string? nivel)
    {
        ViewBag.Progreso = await store.LeerAsync(HttpContext, HttpContext.RequestAborted);
        return View(ActividadesDemo.Crear(NivelDeVista(nivel)));
    }

    [Authorize(Roles = "Alumno"), HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CorregirAventura(string? nivel)
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
        ViewBag.Progreso = await store.GuardarAventuraAsync(HttpContext, resultado, respuestas, HttpContext.RequestAborted);
        return View("ResultadoAventura", resultado);
    }
}
