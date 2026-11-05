# NEXO EDU · MVP en Visual Studio 2022

## Ejecutar
1. Abrir `NEXO-EDU.sln` en Visual Studio y restaurar NuGet.
2. Ejecutar en HTTPS (F5 o Ctrl+F5).
3. La raíz `https://localhost:PUERTO/` abre **NEXO EDU**, ya no el template Welcome.
4. Navegar entre Inicio, Estudiante, Docente, Familia, NEXO PET y Diagnóstico.
5. En Diagnóstico responder las 9 preguntas y consultar el resultado por conceptos.

## Arquitectura
- **ASP.NET Core MVC (.NET 8)** con C#: interfaz MVC responsiva para primer MVP.
- **PostgreSQL + Entity Framework Core**: contexto y entidades preparados.
- **Frontend React/TypeScript**: propuesto para una futura fase; **no está integrado aún**. Primero validamos el flujo completo del MVC existente.
- La mascota se dibuja inicialmente con CSS. Arte 3D, IA generativa y sesiones reales quedan para iteraciones posteriores.

## PostgreSQL local
Crear una base `nexo_edu` en PostgreSQL y configurar `ConnectionStrings:NexoEdu` mediante **Manage User Secrets** (no registrar contraseñas en Git):
```json
{"ConnectionStrings":{"NexoEdu":"Host=localhost;Port=5432;Database=nexo_edu;Username=USUARIO;Password=CLAVE"}}
```
**Importante:** todavía no se incluyen migraciones EF Core ni persistencia de intentos. La aplicación ejecuta la demostración sin PostgreSQL; la conexión es una base técnica preparada, no una base ya operativa.

## Seguridad
- Todas las identidades, calificaciones y alertas que se muestran son **ficticias**.
- El formulario usa token antifalsificación y corrige en el servidor.
- No existe login, roles ni permisos reales. **No desplegar para alumnos reales** hasta implementar autenticación, autorización, consentimiento, protección de datos y auditoría.
- No introducir claves o datos personales en el repositorio.

## Trabajo en equipo
Hacer cambios en ramas feature y abrir pull requests. La rama `master` queda sin modificar hasta revisar y compilar.

## Pendiente en próximos hitos
1. Migraciones iniciales y datos de semilla en PostgreSQL; persistencia por usuario.
2. ASP.NET Core Identity y separación de roles por profesor/estudiante/familia.
3. Alertas reales basadas en evolución, y registro de intervenciones.
4. Tutor que guía sin entregar respuestas directamente, recursos educativos revisados.
5. Pruebas automatizadas, PWA accesible y despliegue seguro.
