using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NEXO_EDU.Models;

namespace NEXO_EDU.Data;

// Al usar un único contexto se guardan los usuarios de Identity y los módulos educativos en PostgreSQL.
public sealed class NexoDbContext(DbContextOptions<NexoDbContext> options) : IdentityDbContext<IdentityUser>(options)
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
        base.OnModelCreating(model); // Obligatorio para configurar Identity (usuarios, roles y contraseñas).
        model.Entity<Estudiante>().HasIndex(x => x.Alias).IsUnique();
        model.Entity<Concepto>().HasIndex(x => x.Clave).IsUnique();
        model.Entity<Pregunta>().HasOne(x => x.Concepto).WithMany().HasForeignKey(x => x.ConceptoId);
        model.Entity<Intento>().HasIndex(x => new { x.EstudianteId, x.PreguntaId, x.FechaUtc });
        model.Entity<AlertaAcademica>().HasIndex(x => new { x.EstudianteId, x.ConceptoId, x.Atendida });
        model.Entity<Mascota>().HasIndex(x => x.EstudianteId).IsUnique();
    }
}
