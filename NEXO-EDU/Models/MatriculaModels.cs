using System.ComponentModel.DataAnnotations;

namespace NEXO_EDU.Models;

public sealed class RegistroDocenteModel
{
    [Required(ErrorMessage="Escribe el nombre completo."), StringLength(120, MinimumLength=3)]
    [RegularExpression(ReglasFormulario.Nombre,ErrorMessage="Usa solo letras, acentos, espacios, guiones o apóstrofes.")]
    [Display(Name="Nombre completo")]
    public string Nombre { get; set; } = "";

    [Required(ErrorMessage="Escribe un correo electrónico."), EmailAddress(ErrorMessage="Introduce un correo válido."), StringLength(256)]
    [Display(Name="Correo institucional o personal")]
    public string Email { get; set; } = "";

    [Required(ErrorMessage="Escribe el nombre de la institución."), StringLength(140, MinimumLength=3)]
    [RegularExpression(ReglasFormulario.Institucion, ErrorMessage="Usa letras, números y signos habituales de nombres de escuelas.")]
    [Display(Name="Institución educativa")]
    public string Institucion { get; set; } = "";

    [Required, RegularExpression("^(primaria|secundaria|preparatoria)$")]
    [Display(Name="Nivel en que impartes clases")]
    public string Nivel { get; set; } = "primaria";

    [Required(ErrorMessage="Escribe una contraseña."), StringLength(128,MinimumLength=12,ErrorMessage="La contraseña debe tener de 12 a 128 caracteres."), DataType(DataType.Password)]
    [Display(Name="Contraseña")]
    public string Password { get; set; } = "";

    [Required(ErrorMessage="Confirma tu contraseña."), Compare(nameof(Password),ErrorMessage="Las contraseñas no coinciden."), DataType(DataType.Password)]
    [Display(Name="Confirmar contraseña")]
    public string ConfirmPassword { get; set; } = "";

    [Required(ErrorMessage="Escribe el código de invitación."), DataType(DataType.Password)]
    [Display(Name="Código de invitación docente")]
    public string CodigoInvitacion { get; set; } = "";
}

public sealed class AltaAlumnoModel : IValidatableObject
{
    [Required(ErrorMessage="Escribe el nombre completo del alumno."), StringLength(120, MinimumLength=2)]
    [RegularExpression(ReglasFormulario.Nombre,ErrorMessage="El nombre solo admite letras, acentos, espacios, guiones o apóstrofes.")]
    [Display(Name="Nombre completo del alumno")]
    public string Nombre { get; set; } = "";

    [Range(1,6,ErrorMessage="Selecciona un grado de 1 a 6.")]
    [Display(Name="Grado")]
    public int Grado { get; set; } = 1;

    [Required(ErrorMessage="Escribe el grupo."), StringLength(12, MinimumLength=1)]
    [RegularExpression(ReglasFormulario.Grupo,ErrorMessage="El grupo solo admite letras y números, sin espacios ni símbolos.")]
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

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (UsuarioManual && !System.Text.RegularExpressions.Regex.IsMatch(
                UsuarioElegido ?? "", ReglasFormulario.Usuario))
            yield return new ValidationResult("Escribe un usuario de 3 a 30 caracteres, letras minúsculas y números; debe comenzar con una letra.",new[]{nameof(UsuarioElegido)});
        if (ClaveManual && !System.Text.RegularExpressions.Regex.IsMatch(
                ClaveElegida ?? "", ReglasFormulario.ClaveAlfanumerica))
            yield return new ValidationResult("Usa de 12 a 64 caracteres con mayúsculas, minúsculas y números, sin espacios ni símbolos.",new[]{nameof(ClaveElegida)});
    }
}

public sealed record AlumnoRegistrado(string AlumnoId, string Nombre, string CorreoAcceso,
    string Nivel, int Grado, string Grupo, int Xp, int Actividades);

public sealed record DocenteGrupo(string Nombre, string Institucion, string Nivel,
    IReadOnlyList<AlumnoRegistrado> Alumnos);

public sealed record CredencialesAlumno(string Nombre, string Correo, string Password,
    string Nivel, int Grado, string Grupo, string? Usuario = null);
