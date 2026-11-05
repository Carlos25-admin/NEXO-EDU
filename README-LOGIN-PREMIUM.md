# NEXO EDU — Renovación visual premium del inicio de sesión

## Alcance
- Pantalla de acceso dividida en dos paneles, con paleta lila, tipografía más clara, tarjetas de perfil y botón de degradado, siguiendo el diseño aprobado.
- NEXO PET animado en el panel izquierdo, construido con elementos HTML y CSS (sin dependencias de imágenes externas): parpadeo, movimiento suave, saludo con un brazo, orejas móviles, estrellas y burbuja que cambia frases.
- Botón para mostrar u ocultar la contraseña sin alterar el envío del formulario.
- Indicadores y estados visuales accesibles en la selección de perfiles. Seleccionar un perfil rellena solamente el correo sugerido; nunca inicia sesión por sí solo y no toca la contraseña.
- Ajustes compartidos a tarjetas, menús, botones y paneles, respetando el diseño individual de primaria, secundaria y preparatoria.
- Adaptación para teléfonos y respeto a preferencias de movimiento reducido.

## Seguridad y compatibilidad
- No se modifica la autenticación de ASP.NET Core Identity, el registro docente, la matrícula ni los permisos de alumno/familia.
- No se cambian los secretos, credenciales, endpoints, tablas, esquemas ni los datos académicos.
- El personaje del login es una ilustración animada mediante CSS; no se añadieron archivos 3D, binarios externos ni llamadas a red.

## Instalación y pruebas
1. Actualiza `feature/base-postgresql-nexo` con Git Pull.
2. No debes ejecutar migraciones SQL para esta mejora visual.
3. Recompila el proyecto y prueba con el perfil `NEXO EDU - PostgreSQL`.
4. Comprueba el acceso a las cinco opciones (primaria, secundaria, prepa, docente y familia) y el inicio de sesión de alumnos por usuario corto.
5. Comprueba que el botón de ojo muestre/oculte la contraseña y que NEXO PET salude, parpadee y cambie su mensaje.
6. Comprueba también en teléfono o vista responsiva del navegador y con movimiento reducido activado.
7. Verifica que las rutas de materias, matrícula docente y seguimiento familiar siguen funcionando.

Este cambio está subido a GitHub, pero queda pendiente compilar y probarlo con Visual Studio en los equipos del proyecto.
