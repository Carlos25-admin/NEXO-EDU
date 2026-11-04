using NEXO_EDU.Models;

namespace NEXO_EDU.Services;

// Configuración visual por nivel; las etapas proceden del XP de la sesión.
public sealed record MascotaEtapa(int Numero, string Nombre, string Descripcion, int XpMinimo);
public sealed record MascotaVisual(string Nivel, string Clave, string Nombre, string Apodo, string Lema, string Color,
    IReadOnlyList<MascotaEtapa> Etapas)
{
    public MascotaEtapa ElegirEtapa(int xp) => Etapas.Last(x => xp >= x.XpMinimo);
    public int SiguienteMeta(int xp) => Etapas.FirstOrDefault(x => x.XpMinimo > xp)?.XpMinimo ?? 360;
}
public static class MascotasNexo
{
    public static MascotaVisual ParaNivel(string? nivel) => NivelDemo.Normalizar(nivel) switch
    {
        NivelDemo.Secundaria => new("secundaria", "coraje", "Coraje", "El compañero valiente",
            "Hasta los héroes tienen miedo. Aprender es intentarlo.", "#43bada", new[]
            {
                new MascotaEtapa(1, "Cachorro curioso", "Un compañero sensible y curioso.", 0),
                new MascotaEtapa(2, "Compañero decidido", "Aprende a enfrentar los retos con valentía.", 70),
                new MascotaEtapa(3, "Héroe del aprendizaje", "Un aliado que celebra cada paso.", 180)
            }),
        NivelDemo.Preparatoria => new("preparatoria", "robot", "NexoBot", "Asistente del futuro",
            "Planea, analiza y desbloquea nuevas habilidades.", "#59dff3", new[]
            {
                new MascotaEtapa(1, "NexoBot Jr.", "Tu primer asistente de estudio.", 0),
                new MascotaEtapa(2, "NexoBot V2", "Un aliado que crece con tus metas.", 70),
                new MascotaEtapa(3, "NexoBot Pro", "Tu mentor tecnológico de aprendizaje.", 180)
            }),
        _ => new("primaria", "aguila", "Nexito", "El águila exploradora",
            "¡Tus alas crecen con cada cosa nueva que aprendes!", "#efb956", new[]
            {
                new MascotaEtapa(1, "Nexito Mini", "Una pequeña águila lista para descubrir.", 0),
                new MascotaEtapa(2, "Nexito Explorador", "Un ave valiente que aprende jugando.", 70),
                new MascotaEtapa(3, "Nexito Guardián", "Un gran compañero con alas de confianza.", 180)
            })
    };
}
