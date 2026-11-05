using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace NEXO_EDU.Data;

// Contexto exclusivo de Identity para conectar a las siete tablas
// public."AspNet*" que ya creó el script de PostgreSQL.
// NO ejecutar EnsureCreated ni InitialCreate sobre EXO-EDU.
public sealed class NexoIdentityDbContext(DbContextOptions<NexoIdentityDbContext> options)
    : IdentityDbContext<IdentityUser>(options)
{
}
