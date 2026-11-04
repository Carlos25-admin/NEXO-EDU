# NEXO EDU · Base técnica del MVP

Rama de integración inicial para Visual Studio 2022, .NET 8 y PostgreSQL.

## Qué contiene
- Proyecto MVC existente conservado (sin sustituir la solución `NEXO-EDU.sln`).
- Pantalla demostrativa en `/NexoDemo` con diagnóstico de fracciones, ruta adaptativa y experiencia de NEXO PET.
- Modelos EF Core y `NexoDbContext` preparados para persistencia con PostgreSQL.
- Lógica de alerta orientativa, NO diagnóstico real.
- Solo disponible en entorno `Development`; sin información de estudiantes reales.

## Requisitos
Visual Studio 2022 con carga ASP.NET/web, .NET SDK 8 y PostgreSQL.

## Configuración de PostgreSQL
1. Crea una base local llamada `nexo_edu` y una cuenta con permisos limitados.
2. En el directorio del proyecto web usa Secret Manager (Visual Studio: botón derecho sobre el proyecto > Administrar secretos de usuario) y guarda:
```json
{
  "ConnectionStrings": {
    "NexoEdu": "Host=localhost;Port=5432;Database=nexo_edu;Username=USUARIO_LOCAL;Password=CLAVE_LOCAL"
  }
}
```
3. No publiques contraseñas en GitHub.
4. El contexto ya está registrado si existe la cadena de conexión; **aún no se han incluido migraciones ni se crean tablas automáticamente**. No se debe afirmar que PostgreSQL ya guarda los resultados.

## Ejecución
1. Abre `NEXO-EDU.sln`, restaura NuGet y compila.
2. Inicia el proyecto en modo `Development`.
3. Abre `/NexoDemo`. El formulario usa datos de ejemplo y NO escribe todavía en la base.

## Pendientes para completar el MVP
- Migración inicial de EF Core y datos semilla educativos.
- Autenticación real con ASP.NET Core Identity y permisos Estudiante/Docente/Familia.
- Guardar intentos, calcular alertas persistentes e intervenciones docentes.
- Integración del frontend React/TypeScript (fase siguiente) y tutor IA guiado.
- Pruebas automatizadas, revisión de accesibilidad y seguridad.

La rama es una propuesta técnica para revisión del equipo. **No usar con datos personales reales ni desplegar en producción.**
