# Solución al error de arranque sin PostgreSQL

El equipo usa, por ahora, la rama `feature/base-postgresql-nexo` y **no tiene PostgreSQL configurado**.

## Configuración corregida
El archivo `Properties/launchSettings.json` incluye como primera opción el perfil **NEXO EDU - Demo local**.

Este perfil establece explícitamente:
- `ASPNETCORE_ENVIRONMENT=Development`
- `DOTNET_ENVIRONMENT=Development`
- `DemoMode__Enabled=true`
- HTTPS y apertura en `/Cuenta/Ingresar`.

Se aplica la misma configuración a los perfiles `https`, `http` e `IIS Express` para evitar que Visual Studio seleccione accidentalmente Identity/PostgreSQL en un arranque local. La variable `DemoMode__Enabled` tiene prioridad sobre el valor anterior de los secretos de usuario.

## Para probar
1. Detener cualquier depuración abierta.
2. Descargar cambios con **Git > Pull** en la rama `feature/base-postgresql-nexo`.
3. En el selector de inicio de Visual Studio (junto al botón verde), elegir **NEXO EDU - Demo local**. No elegir un ejecutable personalizado o un perfil sin las variables de entorno.
4. Ejecutar **Compilar > Recompilar solución**.
5. Ejecutar la aplicación; se abre el login y se utilizan las cuentas de `README-ACCESOS-NIVELES.md`.

En modo Development el sistema seguirá usando usuarios ficticios y sesiones temporales. El código impide activar DemoMode fuera de Development.

## Si aún no aparece el perfil
Cerrar y volver a abrir la solución en Visual Studio. Confirmar que el proyecto `NEXO-EDU` es el proyecto de inicio y que las propiedades de depuración no tienen un perfil personalizado distinto.

## Nota de seguridad
No se han cambiado perfiles, contraseñas, paneles, datos o servicios de producción. No ejecutar públicamente el perfil de Development. No introducir información real de menores.
