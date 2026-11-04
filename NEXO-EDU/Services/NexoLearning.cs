namespace NEXO_EDU.Services;
public static class NexoLearning
{
    // Umbrales iniciales ajustables; no representan un diagnóstico educativo.
    public static string Ruta(int correctas, int total)
    {
        if (total <= 0) return "Diagnóstico pendiente";
        var porcentaje = 100.0 * correctas / total;
        return porcentaje < 50 ? "Refuerzo" : porcentaje < 80 ? "Práctica" : "Avance";
    }

    public static string NivelAlerta(int correctas, int total)
    {
        if (total < 3) return "Sin evidencia suficiente";
        var porcentaje = 100.0 * correctas / total;
        return porcentaje < 50 ? "Rojo" : porcentaje < 80 ? "Amarillo" : "Verde";
    }

    public static int Experiencia(int correctas, int total)
        => total <= 0 ? 0 : total * 10 + correctas * 10;
}
