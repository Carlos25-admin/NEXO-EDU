using System.Text.RegularExpressions;

namespace NEXO_EDU.Models;

/// <summary>
/// Reglas compartidas para formularios escolares.
/// Los nombres admiten tildes, ñ, espacios, guiones y apóstrofes; no números.
/// </summary>
public static class ReglasFormulario
{
    public const string Nombre = @"^[\p{L}\p{M}]+(?:[ '\u2019-][\p{L}\p{M}]+)*$";
    public const string Institucion = @"^(?=.*[\p{L}\p{M}])[\p{L}\p{M}\p{N} .,'\u2019()#&/-]+$";
    public const string Grupo = @"^[\p{L}\p{N}]{1,12}$";
    public const string Usuario = @"^[a-z][a-z0-9]{2,29}$";
    public const string ClaveAlfanumerica = @"^(?=.*[A-Z])(?=.*[a-z])(?=.*[0-9])[A-Za-z0-9]{12,64}$";

    public static string NormalizarEspacios(string? valor) =>
        Regex.Replace((valor ?? "").Trim(), @"\s+", " ");

    public static bool NombreValido(string? valor, int minimo, int maximo)
    {
        var texto = NormalizarEspacios(valor);
        return texto.Length >= minimo && texto.Length <= maximo
            && Regex.IsMatch(texto, Nombre, RegexOptions.CultureInvariant);
    }
}
