using Microsoft.AspNetCore.Mvc;
using NEXO_EDU.Services;

namespace NEXO_EDU.Controllers;

// Solo datos sintéticos para mostrar el flujo; no es un módulo de producción.
public sealed class NexoDemoController(IWebHostEnvironment environment) : Controller
{
    private bool Disponible => environment.IsDevelopment();

    [HttpGet]
    public IActionResult Index()
    {
        if (!Disponible) return NotFound();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Evaluar([FromForm] string? a1, [FromForm] string? a2, [FromForm] string? a3)
    {
        if (!Disponible) return NotFound();
        var respuestas = new[] { a1, a2, a3 };
        var esperadas = new[] { "B", "A", "C" };
        int correctas = respuestas.Where((r, i) => r == esperadas[i]).Count();
        var ruta = NexoLearning.Ruta(correctas, esperadas.Length);
        var nivel = NexoLearning.NivelAlerta(correctas, esperadas.Length);
        var xp = NexoLearning.Experiencia(correctas, esperadas.Length);
        return View("Resultado", new DemoResultado(correctas, esperadas.Length, ruta, nivel, xp));
    }
}

public sealed record DemoResultado(int Correctas, int Total, string Ruta, string NivelAlerta, int Experiencia);
