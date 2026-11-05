using System.ComponentModel.DataAnnotations;

namespace NEXO_EDU.Models;

public sealed class RegistroDocenteModel
{
    [Required, StringLength(120, MinimumLength=3)]
    [Display(Name="Nombre completo")]
    public string Nombre { get; set; } = "";

    [Required, EmailAddress, StringLength(256)]
    [Display(Name="Correo institucional o personal")]
    public string Email { get; set; } = "";

    [Required, StringLength(140, MinimumLength=3)]
    [Display(Name="Institución educativa")]
    public string Institucion { get; set; } = "";

    [Required, RegularExpression("^(primaria|secundaria|preparatoria)$")]
    [Display(Name="Nivel en que impartes clases")]
    public string Nivel { get; set; } = "primaria";

    [Required, MinLength(12), DataType(DataType.Password)]
    [Display(Name="Contraseña")]
    public string Password { get; set; } = "";

    [Required, Compare(nameof(Password)), DataType(DataType.Password)]
    [Display(Name="Confirmar contraseña")]
    public string ConfirmPassword { get; set; } = "";

    [Required, DataType(DataType.Password)]
    [Display(Name="Código de invitación docente")]
    public string CodigoInvitacion { get; set; } = "";
}

public sealed class AltaAlumnoModel
{
    [Required, StringLength(120, MinimumLength=2)]
    [Display(Name="Nombre completo del alumno")]
    public string Nombre { get; set; } = "";

    [Range(1,6)]
    [Display(Name="Grado")]
    public int Grado { get; set; } = 1;

    [Required, StringLength(12, MinimumLength=1)]
    [Display(Name="Grupo")]
    public string Grupo { get; set; } = "A";

    [Display(Name="Escribir usuario manualmente")]
    public bool UsuarioManual { get; set; }

    [StringLength(30)]
    [Display(Name="Usuario de acceso")]
    public string? UsuarioElegido { get; set; }

    [Display(Name="Escribir contraseña manualmente")]
    public bool ClaveManual { get; set; }

    [DataType(DataType.Password)]
    [Display(Name="Contraseña inicial")]
    public string? ClaveElegida { get; set; }
}

public sealed record AlumnoRegistrado(string AlumnoId, string Nombre, string CorreoAcceso,
    string Nivel, int Grado, string Grupo, int Xp, int Actividades);

public sealed record DocenteGrupo(string Nombre, string Institucion, string Nivel,
    IReadOnlyList<AlumnoRegistrado> Alumnos);

public sealed record CredencialesAlumno(string Nombre, string Correo, string Password,
    string Nivel, int Grado, string Grupo, string? Usuario = null);
