using Microsoft.EntityFrameworkCore;
using NEXO_EDU.Models;

namespace NEXO_EDU.Data;

public sealed class NexoDbContext(DbContextOptions<NexoDbContext> options) : DbContext(options)
{
    public DbSet<Estudiante> Estudiantes => Set<Estudiante>();
    public DbSet<Concepto> Conceptos => Set<Concepto>();
    public DbSet<Pregunta> Preguntas => Set<Pregunta>();
    public DbSet<Intento> Intentos => Set<Intento>();
    public DbSet<AlertaAcademica> Alertas => Set<AlertaAcademica>();
    public DbSet<Intervencion> Intervenciones => Set<Intervencion>();
    public DbSet<Mascota> Mascotas => Set<Mascota>();

    protected override void OnModelCreating(ModelBuilder model)
    {
        model.Entity<Estudiante>().HasIndex(x => x.Alias).IsUnique();
        model.Entity<Concepto>().HasIndex(x => x.Clave).IsUnique();
        model.Entity<Pregunta>().HasOne(x => x.Concepto).WithMany().HasForeignKey(x => x.ConceptoId);
        model.Entity<Intento>().HasIndex(x => new { x.EstudianteId, x.PreguntaId, x.FechaUtc });
        model.Entity<AlertaAcademica>().HasIndex(x => new { x.EstudianteId, x.ConceptoId, x.Atendida });
        model.Entity<Mascota>().HasIndex(x => x.EstudianteId).IsUnique();
    }
}
