using System.ComponentModel.DataAnnotations;

namespace NEXO_EDU.Models;

public sealed class BusquedaHijoModel
{
    [Required(ErrorMessage="Escribe el nombre completo del alumno."),StringLength(120,MinimumLength=2)]
    [RegularExpression(ReglasFormulario.Nombre,ErrorMessage="El nombre solo admite letras, acentos, espacios, guiones o apóstrofes.")]
    [Display(Name="Nombre completo del alumno")]
    public string Nombre { get; set; } = "";

    [Required,RegularExpression("^(primaria|secundaria|preparatoria)$",ErrorMessage="Selecciona un nivel válido.")]
    [Display(Name="Nivel educativo")]
    public string Nivel { get; set; } = "primaria";

    [Range(1,6,ErrorMessage="Selecciona un grado válido.")]
    [Display(Name="Grado escolar")]
    public int Grado { get; set; } = 1;
}

public sealed record HijoResumen(
    string AlumnoId,string Nombre,string Nivel,int Grado,string Grupo,
    int Xp,int Actividades,int NivelesSuperados,int UltimoPorcentaje,string? Mascota);

public sealed record HijoIntento(
    string Materia,int Nivel,int Porcentaje,bool Aprobado,DateTime Fecha);

public sealed record HijoMateria(string Clave,int NivelSuperado,int MejorPorcentaje);

public sealed record PanelHijo(HijoResumen Alumno,
    IReadOnlyList<HijoIntento> Intentos,IReadOnlyList<HijoMateria> Materias);

public sealed record PanelFamiliar(
    IReadOnlyList<HijoResumen> Hijos,PanelHijo? Seleccionado,
    string? Aviso=null);
