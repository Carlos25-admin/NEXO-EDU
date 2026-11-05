using System.ComponentModel.DataAnnotations;

namespace NEXO_EDU.Models;

public sealed class LoginViewModel
{
    [Required(ErrorMessage = "Escribe tu usuario o correo electrónico.")]
    [StringLength(256,ErrorMessage="El usuario o correo no puede superar 256 caracteres.")]
    [Display(Name = "Usuario o correo electrónico")]
    public string Email { get; set; } = "";

    [Required(ErrorMessage = "Escribe tu contraseña.")]
    [DataType(DataType.Password)]
    [Display(Name = "Contraseña")]
    public string Password { get; set; } = "";

    public string? ReturnUrl { get; set; }
}
