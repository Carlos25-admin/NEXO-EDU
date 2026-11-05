using Microsoft.AspNetCore.Authorization;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using NEXO_EDU.Models;
using NEXO_EDU.Services;

namespace NEXO_EDU.Controllers;

// Los alumnos de demo tienen nivel fijo por claim. Docentes y familias pueden
// explorar las tres presentaciones sin cambiar de rol ni acceder a datos reales.
public sealed class NexoDemoController(NexoProgressStore store, NexoMatriculaStore matriculas, NexoPetNameStore petNames, NexoFamiliaStore familias, IConfiguration config, IWebHostEnvironment env) : Controller
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

    // Versión provisional estrictamente local: la búsqueda por nombre, grado y
    // nivel NO verifica parentesco y nunca debe estar accesible en producción.
    [Authorize(Roles = "Familia"), HttpGet]
    public async Task<IActionResult> Familia(string? nivel, string? alumnoId)
    {
        ViewBag.Nivel = NivelDeVista(nivel);
        ViewBag.Nombre = User.FindFirst("DemoName")?.Value ?? "Familia";
        if (!env.IsDevelopment() || config.GetValue<bool>("DemoMode:Enabled"))
        {
            ViewBag.FamiliaProvisionalDisabled = true;
            return View(new PanelFamiliar(Array.Empty<HijoResumen>(), null));
        }
        var id = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(id)) return Forbid();
        var hijos = await familias.ListarHijosAsync(id, HttpContext.RequestAborted);
        PanelHijo? elegido = null;
        if (!string.IsNullOrWhiteSpace(alumnoId))
        {
            elegido = await familias.ConsultarAsync(id, alumnoId, HttpContext.RequestAborted);
            if (elegido is null) return Forbid();
        }
        else if (hijos.Count > 0)
            elegido = await familias.ConsultarAsync(id, hijos[0].AlumnoId, HttpContext.RequestAborted);
        return View(new PanelFamiliar(hijos, elegido));
    }

    [Authorize(Roles = "Familia"), HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> BuscarHijo(BusquedaHijoModel model)
    {
        // La búsqueda por datos conocidos es únicamente una funcionalidad de
        // prototipo para personas ficticias en desarrollo.
        if (!env.IsDevelopment() || config.GetValue<bool>("DemoMode:Enabled")) return NotFound();
        model.Nombre = ReglasFormulario.NormalizarEspacios(model.Nombre);
        if (!ReglasFormulario.NombreValido(model.Nombre,2,120))
            ModelState.AddModelError(nameof(model.Nombre),"Escribe un nombre válido, solo letras y espacios.");
        if (model.Nivel != "primaria" && model.Grado > 3)
            ModelState.AddModelError(nameof(model.Grado), "Secundaria y preparatoria tienen grados 1 a 3.");
        if (!ModelState.IsValid)
        {
            TempData["FamiliaError"] = "Revisa nombre completo, nivel educativo y grado.";
            return RedirectToAction(nameof(Familia));
        }
        var id = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(id)) return Forbid();
        var resultado = await familias.VincularPorDatosAsync(id, model, HttpContext.RequestAborted);
        TempData[resultado == 1 ? "FamiliaOk" : "FamiliaError"] = resultado switch
        {
            1 => "Encontramos el registro. Ya puedes consultar el seguimiento de este alumno de prueba.",
            -2 => "Encontramos más de un alumno con esos datos; pide al docente una vinculación individual.",
            _ => "No encontramos una coincidencia exacta. Revisa el nombre, el grado y el nivel."
        };
        return RedirectToAction(nameof(Familia), new { nivel = model.Nivel });
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
        var elegido = ReglasFormulario.NormalizarEspacios(nombre);
        // Nombres cortos legibles en español; sin caracteres de marcado ni control.
        if (elegido.Length is < 2 or > 24 ||
            !Regex.IsMatch(elegido, ReglasFormulario.Nombre))
        {
            TempData["PetNameError"] = "Elige un nombre de 2 a 24 caracteres usando solo letras, acentos y espacios.";
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
