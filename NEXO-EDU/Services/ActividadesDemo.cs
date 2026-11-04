using NEXO_EDU.Models;

namespace NEXO_EDU.Services;

public sealed record RetoDemo(int Numero, string Enunciado, string[] Opciones, int Correcta, string Explicacion);
public sealed record AventuraDemo(string Nivel, string Titulo, string Materia, string Descripcion, IReadOnlyList<RetoDemo> Retos);
public sealed record ResultadoAventuraDemo(string Nivel, int Correctas, int Total, int Xp)
{
    public int Porcentaje => Total == 0 ? 0 : 100 * Correctas / Total;
}

public static class ActividadesDemo
{
    public static AventuraDemo Crear(string? nivel)
    {
        var n = NivelDemo.Normalizar(nivel);
        return n switch
        {
            NivelDemo.Secundaria => new(n, "Exploración de fracciones", "Matemáticas", "Tres retos para identificar lo que ya dominas.",
                new[]
                {
                    new RetoDemo(1, "¿Cuánto es 1/2 + 1/4?", new[] { "2/6", "3/4", "1/4" }, 1, "Usa denominador común: 2/4 + 1/4 = 3/4."),
                    new RetoDemo(2, "Simplifica 6/8.", new[] { "3/4", "2/4", "6/4" }, 0, "Divide numerador y denominador entre 2."),
                    new RetoDemo(3, "¿Cuál es 25% como fracción?", new[] { "1/2", "1/3", "1/4" }, 2, "25/100 equivale a 1/4.")
                }),
            NivelDemo.Preparatoria => new(n, "Tu desafío de funciones", "Matemáticas", "Aplica conceptos de álgebra con una evaluación breve.",
                new[]
                {
                    new RetoDemo(1, "Si f(x)=2x+3, ¿cuánto vale f(4)?", new[] { "11", "9", "8" }, 0, "Sustituye x=4: 2(4)+3=11."),
                    new RetoDemo(2, "Resuelve 3x-6=9.", new[] { "3", "5", "7" }, 1, "Suma 6 y divide entre 3."),
                    new RetoDemo(3, "La pendiente de y=−2x+1 es:", new[] { "1", "2", "−2" }, 2, "La pendiente es el coeficiente de x.")
                }),
            _ => new(n, "El bosque de los números", "Matemáticas", "Cuenta, juega y descubre nuevas respuestas.",
                new[]
                {
                    new RetoDemo(1, "En el bosque hay 3 mariposas y llegan 2. ¿Cuántas hay?", new[] { "5", "4", "6" }, 0, "Tres más dos es igual a cinco."),
                    new RetoDemo(2, "Leo tiene 8 semillas y planta 3. ¿Cuántas le quedan?", new[] { "6", "5", "4" }, 1, "Ocho menos tres es cinco."),
                    new RetoDemo(3, "¿Qué número continúa? 2, 4, 6, …", new[] { "7", "10", "8" }, 2, "La secuencia aumenta de dos en dos.")
                })
        };
    }

    public static ResultadoAventuraDemo Evaluar(string? nivel, IReadOnlyDictionary<int, int> respuestas)
    {
        var aventura = Crear(nivel);
        var aciertos = aventura.Retos.Count(r => respuestas.TryGetValue(r.Numero, out var respuesta) && respuesta == r.Correcta);
        // Todos los intentos otorgan XP, sin castigos por equivocarse.
        return new ResultadoAventuraDemo(aventura.Nivel, aciertos, aventura.Retos.Count, 15 + aciertos * 15);
    }
}
