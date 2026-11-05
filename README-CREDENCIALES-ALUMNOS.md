# NEXO EDU · Credenciales amigables para estudiantes

## Qué cambia
- Cada docente puede dejar **usuario y contraseña automáticos**, escribir **ambos manualmente** o utilizar una mezcla (usuario manual + contraseña automática, y viceversa).
- Usuario automático: primera palabra y última palabra del nombre normalizadas (minúsculas, sin acentos) + cuatro números aleatorios, por ejemplo `carlosramirez4821`. Se verifica disponibilidad antes de crear la cuenta.
- Usuario manual: 3 a 30 caracteres, solamente letras minúsculas y números; debe empezar por letra. Ejemplo: `carlosramirez25`.
- Contraseña automática: 16 caracteres aleatorios criptográficos, que incluyen al menos una mayúscula, una minúscula y un número; sin símbolos.
- Contraseña manual: de 12 a 64 caracteres, con mayúscula, minúscula y número, solamente caracteres alfanuméricos. No uses nombres completos, cumpleaños o claves fáciles.
- El formulario asigna a cada nuevo alumno `IdentityUser.UserName` con ese identificador; el correo técnico reservado en `IdentityUser.Email` queda `<usuario>@alumnos.nexoedu.test`. Ningún correo de un menor es necesario.
- El login acepta **usuario corto** para nuevos alumnos o **correo completo** para las cuentas existentes. También se puede usar el correo técnico como identificador de acceso.
- El docente solo ve la contraseña durante la creación o cuando la restablece; se almacena hash mediante ASP.NET Identity, no texto en PostgreSQL.
- Al restablecer claves desde Mis alumnos, la nueva contraseña también será alfanumérica de 16 caracteres.
- Los usuarios anteriores y sus contraseñas no se modifican. Todos los avances, docentes, alumnos, grupos y XP persisten.

## Instalación
1. Git → Pull en `feature/base-postgresql-nexo`.
2. NO ejecutar ninguna migración nueva; el cambio aprovecha las columnas existentes `AspNetUsers.UserName` y `AspNetUsers.Email`.
3. Mantener `PostgresBootstrap:Enabled=false` y el perfil `NEXO EDU - PostgreSQL`.
4. Compilar la solución y arrancar F5.
5. Como docente, abrir **Mis alumnos → Registrar alumno**. Probar un alumno con credenciales automáticas y otro con usuario y contraseña manuales.
6. Ingresar con los nuevos usuarios cortos, completar una actividad y verificar que el progreso siga guardándose.
7. Comprobar el rechazo de usuarios repetidos y contraseñas manuales incompletas.
8. Entrar con alguno de los cinco correos de prueba anteriores: deben seguir funcionando.

## Seguridad
Para permitir contraseñas alfanuméricas, el requisito global de símbolo de Identity se dejó opcional (`RequireNonAlphanumeric=false`), pero siguen siendo obligatorias las mayúsculas, minúsculas, números y longitud mínima de 12. Aconsejamos usar contraseñas automáticas o contraseñas manuales largas, y mantener los bloqueos por intentos fallidos.

Los nombres completos y credenciales de menores no deben circular por canales públicos. En la versión destinada a escuelas reales harán falta consentimiento familiar, política de entrega de contraseñas, cambio obligatorio en primer acceso, verificación de docentes e inspección de controles de acceso.

## Limitaciones
Código subido para revisión; no se ha compilado ni probado en las laptops del equipo desde este entorno.
