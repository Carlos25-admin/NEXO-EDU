using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace NEXO_EDU.Services;

// Solo almacena resultados ficticios dentro de la sesión del navegador. No es un expediente escolar.
public sealed class ProgresoDemo
{
    public Dictionary<string, int> NivelesSuperados { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public int Xp { get; set; }
    public int Actividades { get; set; }
    public int UltimoPorcentaje { get; set; }
    public string? UltimaRuta { get; set; }
    public string? UltimaMateria { get; set; }
    public string? Mensaje { get; set; }
    public int Etapa => Xp >= 180 ? 3 : Xp >= 70 ? 2 : 1;
}
public static class DemoProgressService
{
    private static string Key(string? email) => "NexoProgress:" + (email ?? "anonymous").Trim().ToLowerInvariant();

    public static ProgresoDemo Leer(ISession session, string? email)
    {
        var json = session.GetString(Key(email));
        if (string.IsNullOrWhiteSpace(json)) return new ProgresoDemo();
        try { return JsonSerializer.Deserialize<ProgresoDemo>(json) ?? new ProgresoDemo(); }
        catch (JsonException) { return new ProgresoDemo(); }
    }

    // Avance lineal por módulo; solo se recompensa la primera aprobación.
    public static int MaximoDesbloqueado(ProgresoDemo progreso, string clave)
    {
        progreso.NivelesSuperados ??= new(StringComparer.OrdinalIgnoreCase);
        return Math.Min(5, progreso.NivelesSuperados.GetValueOrDefault(clave, 0) + 1);
    }

    public static bool AprobarNivel(ISession session, string? email, string clave, int nivel,
        ResultadoAventuraDemo resultado, string materia)
    {
        var progreso = Leer(session, email);
        progreso.NivelesSuperados ??= new(StringComparer.OrdinalIgnoreCase);
        var completado = progreso.NivelesSuperados.GetValueOrDefault(clave, 0);
        if (nivel < 1 || nivel > 5 || nivel > completado + 1 || nivel <= completado) return false;
        progreso.NivelesSuperados[clave] = nivel;
        // Registrar es el único punto que suma XP y guarda la sesión.
        session.SetString(Key(email), JsonSerializer.Serialize(progreso));
        Registrar(session, email, resultado, materia);
        return true;
    }

    public static ProgresoDemo Registrar(ISession session, string? email, ResultadoAventuraDemo resultado, string? materia = null)
    {
        var actual = Leer(session, email);
        actual.Xp += resultado.Xp;
        actual.Actividades++;
        actual.UltimoPorcentaje = resultado.Porcentaje;
        actual.UltimaRuta = resultado.Porcentaje < 50 ? "Refuerzo guiado" :
            resultado.Porcentaje < 80 ? "Práctica complementaria" : "Nuevo desafío";
        actual.UltimaMateria = materia ?? "Matemáticas";
        actual.Mensaje = resultado.Porcentaje < 50
            ? "¡Buen intento! Repasemos este tema de otra manera."
            : resultado.Porcentaje < 80
                ? "¡Estás avanzando! La práctica te ayudará a dominarlo."
                : "¡Excelente avance! Ya puedes intentar un reto nuevo.";
        session.SetString(Key(email), JsonSerializer.Serialize(actual));
        return actual;
    }
}
