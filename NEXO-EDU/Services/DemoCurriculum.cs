namespace NEXO_EDU.Services;

// Banco educativo ficticio y verificable para el hackatón. Las respuestas nunca se imprimen en HTML.
public sealed record DemoQuestion(int Id, string Concepto, string Enunciado, string[] Opciones, int Correcta, string Pista, string Explicacion);
public sealed record ConceptScore(string Concepto, int Correctas, int Total, string Ruta, string Semaforo);
public sealed record DemoEvaluation(int Correctas, int Total, int Experiencia, string Ruta, string Semaforo, IReadOnlyList<ConceptScore> Conceptos)
{
    public int Porcentaje => Total == 0 ? 0 : (int)Math.Round(100.0 * Correctas / Total);
    public int EtapaMascota => Experiencia >= 180 ? 3 : Experiencia >= 70 ? 2 : 1;
}
public static class DemoCurriculum
{
    public static readonly IReadOnlyList<DemoQuestion> Preguntas = new[]
    {
        new DemoQuestion(1, "Equivalencia", "¿Qué fracción equivale a 2/4?", new[] { "1/4", "1/2", "3/4" }, 1, "Simplifica numerador y denominador entre dos.", "2/4 equivale a 1/2 porque ambos términos se dividen entre 2."),
        new DemoQuestion(2, "Suma", "¿Cuánto es 1/2 + 1/2?", new[] { "1", "1/2", "2" }, 0, "Suma las dos mitades de una unidad.", "Dos mitades completan una unidad."),
        new DemoQuestion(3, "Resta", "¿Cuánto es 3/4 − 1/4?", new[] { "1/4", "3/4", "1/2" }, 2, "Con igual denominador, resta solamente numeradores.", "3/4 menos 1/4 son 2/4, que se simplifican a 1/2."),
        new DemoQuestion(4, "Equivalencia", "¿Cuál de estas fracciones equivale a 3/6?", new[] { "1/2", "2/3", "1/3" }, 0, "Prueba dividir arriba y abajo entre tres.", "3/6 se simplifica a 1/2."),
        new DemoQuestion(5, "Suma", "¿Cuánto es 1/4 + 2/4?", new[] { "3/8", "3/4", "1/2" }, 1, "Mantén el denominador y suma los numeradores.", "1/4 más 2/4 es 3/4."),
        new DemoQuestion(6, "Resta", "¿Cuánto es 5/8 − 2/8?", new[] { "3/8", "3/16", "7/8" }, 0, "Ambas fracciones tienen el mismo denominador.", "5/8 menos 2/8 es 3/8."),
        new DemoQuestion(7, "Equivalencia", "¿Qué fracción representa 75%?", new[] { "1/2", "1/4", "3/4" }, 2, "75/100 puede simplificarse entre 25.", "75% es 75/100, equivalente a 3/4."),
        new DemoQuestion(8, "Suma", "¿Cuánto es 2/3 + 1/3?", new[] { "1", "3/6", "2/3" }, 0, "Piensa en tres tercios que forman un entero.", "2/3 más 1/3 completan 3/3 = 1."),
        new DemoQuestion(9, "Resta", "¿Cuánto es 7/10 − 3/10?", new[] { "4/20", "4/10", "10/10" }, 1, "Resta los numeradores y conserva el denominador.", "7/10 menos 3/10 es 4/10, equivalente a 2/5.")
    };
    public static DemoEvaluation Evaluar(IReadOnlyDictionary<int,int> respuestas)
    {
        var aciertos = Preguntas.Count(p => respuestas.TryGetValue(p.Id, out var valor) && valor == p.Correcta);
        var conceptos = Preguntas.GroupBy(p => p.Concepto)
            .Select(g =>
            {
                var bien = g.Count(p => respuestas.TryGetValue(p.Id, out var valor) && valor == p.Correcta);
                return new ConceptScore(g.Key, bien, g.Count(),
                    NexoLearning.Ruta(bien, g.Count()), NexoLearning.NivelAlerta(bien, g.Count()));
            }).ToArray();
        var xp = NexoLearning.Experiencia(aciertos, Preguntas.Count);
        return new DemoEvaluation(aciertos, Preguntas.Count(), xp,
            NexoLearning.Ruta(aciertos, Preguntas.Count()),
            NexoLearning.NivelAlerta(aciertos, Preguntas.Count()), conceptos);
    }
}
