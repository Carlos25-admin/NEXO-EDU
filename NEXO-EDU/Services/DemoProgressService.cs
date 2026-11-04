using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace NEXO_EDU.Services;

// Solo almacena resultados ficticios dentro de la sesión del navegador. No es un expediente escolar.
public sealed class ProgresoDemo
{
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

    public static ProgresoDemo Registrar(ISession session, string? email, ResultadoAventuraDemo resultado)
    {
        var actual = Leer(session, email);
        actual.Xp += resultado.Xp;
        actual.Actividades++;
        actual.UltimoPorcentaje = resultado.Porcentaje;
        actual.UltimaRuta = resultado.Porcentaje < 50 ? "Refuerzo guiado" :
            resultado.Porcentaje < 80 ? "Práctica complementaria" : "Nuevo desafío";
        actual.UltimaMateria = "Matemáticas";
        actual.Mensaje = resultado.Porcentaje < 50
            ? "¡Buen intento! Repasemos este tema de otra manera."
            : resultado.Porcentaje < 80
                ? "¡Estás avanzando! La práctica te ayudará a dominarlo."
                : "¡Excelente avance! Ya puedes intentar un reto nuevo.";
        session.SetString(Key(email), JsonSerializer.Serialize(actual));
        return actual;
    }
}
