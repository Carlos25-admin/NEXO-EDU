using NEXO_EDU.Models;

namespace NEXO_EDU.Services;

public sealed record RetoDemo(int Numero, string Enunciado, string[] Opciones, int Correcta, string Explicacion);
public sealed record AventuraDemo(string Nivel, string Titulo, string Materia, string Descripcion, IReadOnlyList<RetoDemo> Retos);
public sealed record RetroalimentacionDemo(int Numero, string Enunciado, string RespuestaCorrecta, string Explicacion, bool Correcta);
public sealed record ResultadoAventuraDemo(string Nivel, int Correctas, int Total, int Xp, IReadOnlyList<RetroalimentacionDemo> Detalles)
{
    public int Porcentaje => Total == 0 ? 0 : (int)Math.Round(100.0 * Correctas / Total);
}
public static class ActividadesDemo
{
    public static AventuraDemo Crear(string? nivel)
    {
        var n = NivelDemo.Normalizar(nivel);
        return n switch
        {
            NivelDemo.Secundaria => new(n, "Exploración de fracciones", "Matemáticas",
                "Resuelve seis retos de equivalencias, operaciones y porcentajes.",
                new[]
                {
                    new RetoDemo(1, "¿Cuánto es 1/2 + 1/4?", new[] { "2/6", "3/4", "1/4" }, 1, "Usa denominador común: 2/4 + 1/4 = 3/4."),
                    new RetoDemo(2, "Simplifica 6/8.", new[] { "3/4", "2/4", "6/4" }, 0, "Divide numerador y denominador entre 2."),
                    new RetoDemo(3, "¿Cuál es 25% como fracción?", new[] { "1/2", "1/3", "1/4" }, 2, "25/100 equivale a 1/4."),
                    new RetoDemo(4, "¿Cuánto es 3/5 + 1/5?", new[] { "4/10", "4/5", "3/10" }, 1, "Si el denominador es igual, suma numeradores: 4/5."),
                    new RetoDemo(5, "¿Qué decimal equivale a 3/4?", new[] { "0.75", "0.25", "0.50" }, 0, "Tres dividido entre cuatro es 0.75."),
                    new RetoDemo(6, "¿Cuánto es 2/3 − 1/3?", new[] { "1/6", "3/3", "1/3" }, 2, "Dos tercios menos un tercio es un tercio.")
                }),
            NivelDemo.Preparatoria => new(n, "Funciones y álgebra aplicada", "Matemáticas",
                "Aplica seis conceptos fundamentales de álgebra y funciones.",
                new[]
                {
                    new RetoDemo(1, "Si f(x)=2x+3, ¿cuánto vale f(4)?", new[] { "11", "9", "8" }, 0, "Sustituye x=4: 2(4)+3=11."),
                    new RetoDemo(2, "Resuelve 3x−6=9.", new[] { "3", "5", "7" }, 1, "Suma 6 a ambos lados y divide entre 3."),
                    new RetoDemo(3, "La pendiente de y=−2x+1 es:", new[] { "1", "2", "−2" }, 2, "La pendiente es el coeficiente de x."),
                    new RetoDemo(4, "Si f(x)=x², ¿cuánto vale f(−3)?", new[] { "9", "−9", "6" }, 0, "Elevar −3 al cuadrado produce 9."),
                    new RetoDemo(5, "La solución de x²=16 es:", new[] { "Solo 4", "4 y −4", "Solo −4" }, 1, "Ambos valores elevados al cuadrado son 16."),
                    new RetoDemo(6, "Si la pendiente es 0, la recta es:", new[] { "Vertical", "Inclinada", "Horizontal" }, 2, "Una recta horizontal mantiene constante su valor de y.")
                }),
            _ => new(n, "El bosque de los números", "Matemáticas",
                "Seis pequeños retos para aprender jugando.",
                new[]
                {
                    new RetoDemo(1, "En el bosque hay 3 mariposas y llegan 2. ¿Cuántas hay?", new[] { "5", "4", "6" }, 0, "Tres más dos es igual a cinco."),
                    new RetoDemo(2, "Leo tiene 8 semillas y planta 3. ¿Cuántas le quedan?", new[] { "6", "5", "4" }, 1, "Ocho menos tres es cinco."),
                    new RetoDemo(3, "¿Qué número continúa? 2, 4, 6, …", new[] { "7", "10", "8" }, 2, "La secuencia aumenta de dos en dos."),
                    new RetoDemo(4, "¿Cuánto es 7 + 2?", new[] { "8", "9", "10" }, 1, "Cuenta dos números después de siete: ocho y nueve."),
                    new RetoDemo(5, "En una caja hay 10 lápices. Sacas 4. ¿Cuántos quedan?", new[] { "6", "5", "7" }, 0, "Diez menos cuatro es seis."),
                    new RetoDemo(6, "¿Cuál es el número mayor?", new[] { "12", "15", "9" }, 1, "Quince es mayor que doce y nueve.")
                })
        };
    }

    public static ResultadoAventuraDemo Evaluar(string? nivel, IReadOnlyDictionary<int, int> respuestas)
    {
        var actividad = Crear(nivel);
        var detalles = actividad.Retos.Select(reto =>
            new RetroalimentacionDemo(reto.Numero, reto.Enunciado,
                reto.Opciones[reto.Correcta], reto.Explicacion,
                respuestas.TryGetValue(reto.Numero, out var opcion) && opcion == reto.Correcta)).ToArray();
        var aciertos = detalles.Count(d => d.Correcta);
        return new ResultadoAventuraDemo(actividad.Nivel, aciertos, actividad.Retos.Count,
            15 + aciertos * 10, detalles);
    }
}
