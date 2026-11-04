namespace NEXO_EDU.Models;

// Entidades iniciales para el MVP con identidades ficticias.
// No se deben almacenar datos de menores reales sin permisos, autenticación y aviso de privacidad.
public sealed class Estudiante
{
    public int Id { get; set; }
    public string Alias { get; set; } = "";
    public string Grado { get; set; } = "Secundaria";
}
public sealed class Concepto
{
    public int Id { get; set; }
    public string Clave { get; set; } = "";
    public string Nombre { get; set; } = "";
}
public sealed class Pregunta
{
    public int Id { get; set; }
    public int ConceptoId { get; set; }
    public Concepto Concepto { get; set; } = null!;
    public string Enunciado { get; set; } = "";
    public string OpcionA { get; set; } = "";
    public string OpcionB { get; set; } = "";
    public string OpcionC { get; set; } = "";
    public string RespuestaCorrecta { get; set; } = "A";
    public string Explicacion { get; set; } = "";
    public int Dificultad { get; set; } = 1;
}
public sealed class Intento
{
    public int Id { get; set; }
    public int EstudianteId { get; set; }
    public int PreguntaId { get; set; }
    public string Respuesta { get; set; } = "";
    public bool Correcta { get; set; }
    public DateTime FechaUtc { get; set; } = DateTime.UtcNow;
}
public sealed class AlertaAcademica
{
    public int Id { get; set; }
    public int EstudianteId { get; set; }
    public int ConceptoId { get; set; }
    public string Motivo { get; set; } = "";
    public string Nivel { get; set; } = "Amarillo";
    public bool Atendida { get; set; }
    public DateTime FechaUtc { get; set; } = DateTime.UtcNow;
}
public sealed class Intervencion
{
    public int Id { get; set; }
    public int AlertaAcademicaId { get; set; }
    public string Accion { get; set; } = "";
    public string Resultado { get; set; } = "";
    public DateTime FechaUtc { get; set; } = DateTime.UtcNow;
}
public sealed class Mascota
{
    public int Id { get; set; }
    public int EstudianteId { get; set; }
    public string Nombre { get; set; } = "Nexi";
    public int Experiencia { get; set; }
    public int Etapa => Experiencia >= 180 ? 3 : Experiencia >= 70 ? 2 : 1;
}
