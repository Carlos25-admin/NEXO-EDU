namespace NEXO_EDU.Services;

// Progreso por materia y nivel dentro de la sesión, separado por cuenta.
public sealed record RutaNivelViewModel(LeccionNexo Modulo, int NivelMaximo, int TotalNiveles, int PreguntasDisponibles);
public sealed record ActividadNivelViewModel(LeccionNexo Modulo, int Nivel, IReadOnlyList<PreguntaNexo> Preguntas);
public sealed record ResultadoNivelViewModel(RevisionLeccionNexo Revision, int Nivel, bool Aprobado, bool RecompensaNueva, int MejorNivel, int PuntosOtorgados)
{
    public int Siguiente => Math.Min(5, Nivel + 1);
}
