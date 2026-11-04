# NEXO EDU — Inicio de sesión por perfiles (MVP)

Este módulo integra **ASP.NET Core Identity + PostgreSQL** en la solución MVC de Visual Studio 2022 (.NET 8).

## Preparar una base NUEVA de pruebas
1. Instala PostgreSQL y crea una base de datos local NUEVA y VACÍA llamada `nexo_edu_login_dev`. No uses la base de producción ni una con datos importantes.
2. En Visual Studio: clic derecho en el proyecto **NEXO-EDU** > **Administrar secretos de usuario**.
3. Añade la configuración privada de la base, las tres identidades de prueba y sus contraseñas. No subas el archivo de secretos al repositorio:
```json
{
  "ConnectionStrings": {
    "NexoEdu": "Host=localhost;Port=5432;Database=nexo_edu_login_dev;Username=postgres;Password=CAMBIAR_POR_CLAVE_POSTGRES"
  },
  "DemoUsers": {
    "Initialize": true,
    "Alumno": { "Email": "alumno@nexoedu.test", "Password": "COLOCA_LA_CLAVE_DEL_ALUMNO" },
    "Docente": { "Email": "docente@nexoedu.test", "Password": "COLOCA_LA_CLAVE_DEL_DOCENTE" },
    "Familia": { "Email": "familia@nexoedu.test", "Password": "COLOCA_LA_CLAVE_DE_FAMILIA" }
  }
}
```
4. Sustituye las contraseñas de ejemplo por las tres claves privadas que se entregan por separado al responsable del equipo.
5. Ejecuta la aplicación (perfil HTTPS, entorno Development). El inicializador crea las tablas de Identity y las cuentas **solo si la base estaba vacía**.
6. Entra a `https://localhost:7189/Cuenta/Ingresar` y prueba un perfil a la vez. La ruta y el puerto pueden variar según el perfil de Visual Studio.
7. Si necesitas detener el inicializador tras crear las cuentas, cambia `DemoUsers:Initialize` a `false`. Las cuentas persisten en PostgreSQL y el inicio de sesión sigue funcionando.

## Accesos
| Perfil | Correo de prueba | Sección permitida |
| --- | --- | --- |
| Alumno | `alumno@nexoedu.test` | /NexoDemo/Estudiante, /NexoDemo/Mascota, /NexoDemo/Diagnostico |
| Docente | `docente@nexoedu.test` | /NexoDemo/Docente |
| Familia | `familia@nexoedu.test` | /NexoDemo/Familia |

### Importante
- La ruta `/` continúa siendo pública y muestra la página de presentación. Los paneles y el diagnóstico requieren sesión con el rol correspondiente.
- **No se permite el registro público**. Las cuentas se crean de manera controlada y las contraseñas se almacenan mediante hashing de Identity.
- Se usa cookie `HttpOnly` y `Secure`, tokens antifalsificación en formularios y bloqueo temporal tras cinco intentos fallidos.
- Las pantallas docentes y familiares todavía utilizan datos ficticios para demostración.
- El inicializador emplea `EnsureCreatedAsync` solo en DEVELOPMENT para una base nueva vacía. **No es una migración** y no sirve para actualizar una base existente. En producción se necesitarán migraciones EF Core, política de contraseñas, recuperación de acceso, auditoría y protección de datos de menores.
- Sin PostgreSQL y los secretos configurados, **la aplicación no puede iniciar sesión ni arrancar correctamente**. Esto es intencional para evitar simular seguridad sin una base real.
