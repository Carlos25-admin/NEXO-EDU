namespace NEXO_EDU.Models;

// El selector es exclusivo del prototipo: posteriormente cada perfil
// recibirá su nivel y grupo desde PostgreSQL, no desde un parámetro URL.
public static class NivelDemo
{
    public const string Primaria = "primaria";
    public const string Secundaria = "secundaria";
    public const string Preparatoria = "preparatoria";

    public static string Normalizar(string? nivel) => nivel?.Trim().ToLowerInvariant() switch
    {
        Secundaria => Secundaria,
        Preparatoria => Preparatoria,
        _ => Primaria
    };

    public static string Nombre(string nivel) => nivel switch
    {
        Secundaria => "Secundaria",
        Preparatoria => "Preparatoria",
        _ => "Primaria"
    };
}
