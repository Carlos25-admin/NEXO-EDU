# NEXO EDU — Inicio de sesión por perfiles (MVP)

## Fase actual: desarrollar SIN PostgreSQL
Este proyecto permite probar los módulos de **Alumno**, **Docente** y **Familia** desde Visual Studio 2022 sin instalar una base de datos.

1. En Visual Studio, abre `NEXO-EDU.sln` y selecciona el perfil **https**.
2. Haz clic derecho en el proyecto `NEXO-EDU` → **Administrar secretos de usuario**.
3. Deja **sin configurar** la entrada `ConnectionStrings:NexoEdu`: elimina esa sección o usa una cadena vacía. Si existe una conexión configurada, el proyecto intentará acceder a PostgreSQL.
4. Guarda los siguientes valores locales (sustituye las claves por las contraseñas privadas de prueba del equipo). **Nunca subas secrets.json a GitHub.**
```json
{
  "DemoUsers": {
    "Alumno": {
      "Email": "alumno@nexoedu.test",
      "Password": "CLAVE_PRIVADA_ALUMNO"
    },
    "Docente": {
      "Email": "docente@nexoedu.test",
      "Password": "CLAVE_PRIVADA_DOCENTE"
    },
    "Familia": {
      "Email": "familia@nexoedu.test",
      "Password": "CLAVE_PRIVADA_FAMILIA"
    }
  }
}
```
5. Detén y vuelve a iniciar Visual Studio con **F5**. La portada estará en `/` y el acceso en `/Cuenta/Ingresar`.
6. Verifica que Alumno entra en su panel, Docente en el panel docente y Familia en el familiar. Los módulos tienen `[Authorize(Roles = "...")]`.

**Seguridad del modo sin base de datos:** únicamente funciona con `ASPNETCORE_ENVIRONMENT=Development` y cuando NO hay cadena de PostgreSQL. Las contraseñas están en User Secrets en cada computadora, no en el repositorio. Se utiliza una cookie de sesión local HTTPS con protección antifalsificación. No tiene persistencia de usuarios, bloqueo de intentos, recuperación de contraseña ni garantía de seguridad de producción. No usar con estudiantes reales ni publicar a Internet.

## Fase posterior: activar PostgreSQL + ASP.NET Core Identity
- Crear base nueva y vacía `nexo_edu_login_dev`.
- Añadir en User Secrets `ConnectionStrings:NexoEdu` y las contraseñas de prueba.
- Para inicializar la base nueva, establecer `DemoUsers:Initialize=true` **solo en Development**.
- Identity administra hashes de contraseñas, bloqueo tras intentos fallidos, roles y sesiones. La inicialización mediante EnsureCreated es solo temporal: antes del despliegue se requieren migraciones EF Core.
- En producción se requiere PostgreSQL, auditoría, seguridad adicional y protección de datos de menores.

## Desarrollo en equipo
Cada integrante guarda las mismas cuentas de ejemplo y sus propias configuraciones localmente. Los datos de demostración se encuentran definidos en el código y no representan calificaciones reales. No hagan Merge hacia `master` sin compilar y revisar la rama.
