# NEXO EDU · Inscripción por docentes (migración 005)

## Qué se implementó
- El **docente** puede crear una cuenta desde `/Matricula/RegistroDocente` **únicamente con un código de invitación privado** configurado por quien administra la institución. El nivel queda vinculado a su cuenta (Primaria, Secundaria o Preparatoria).
- Desde su panel accede a **Mis alumnos** y **Registrar alumno**.
- El registro de estudiante solicita únicamente **nombre, grado y grupo**. El nivel se toma del perfil docente, jamás de un dato enviado por el alumno.
- Al guardar, Identity crea un usuario individual con correo de acceso virtual `nexo-...@alumnos.nexoedu.test` y una contraseña fuerte de 18 caracteres; el docente la ve **una sola vez**.
- El docente consulta los estudiantes a su cargo y sus estadísticas de XP e intentos reales guardadas desde la migración 004.
- Si una contraseña se extravía, el docente puede generar otra exclusivamente para **sus propios estudiantes**.
- Los estudiantes **no** tienen registro público. El login existente acepta el usuario virtual y contraseña; el nivel de los alumnos se fija mediante claim del lado del servidor.
- Los indicadores ficticios anteriores se presentan como ilustrativos, mientras la sección de grupo muestra datos verdaderos del prototipo.

## Instalación necesaria (cada laptop)
1. Hacer copia de seguridad de `EXO-EDU` en pgAdmin (formato Custom).
2. Descargar la rama `feature/base-postgresql-nexo` con Git Pull.
3. Ejecutar en **EXO-EDU** el script **`NEXO-EDU/Sql/005_inscripciones_docente.sql`** completo. Es aditivo, no elimina registros y no modifica las 51 tablas existentes.
4. Verificar:
```sql
SELECT to_regclass('nexo.mvp_docentes') AS docentes,
       to_regclass('nexo.mvp_alumnos_docente') AS alumnos;
```
5. En Visual Studio → Administrar secretos de usuario, conservar la cadena `ConnectionStrings:NexoEdu`. Dejar `PostgresBootstrap:Enabled=false`. Agregar un código nuevo **largo, aleatorio, único por institución/entorno y de al menos 20 caracteres**:
```json
{
  "RegistroDocente": {
    "Codigo": "SUSTITUIR_POR_CODIGO_LARGO_GENERADO_LOCALMENTE"
  }
}
```
**No reemplazar el resto de secrets.json**; integrar únicamente el bloque RegistroDocente. No publicar ese secreto en GitHub ni conversaciones. Distribuir el código a docentes verificados por un canal confiable.

6. Compilar (Ctrl+Shift+B), usar perfil **NEXO EDU - PostgreSQL**, abrir la pantalla de login y seleccionar **Crear cuenta docente con invitación**.
7. Crear la cuenta del docente, iniciar sesión con ella, entrar a **Mis alumnos** y registrar un alumno. Guardar la contraseña generada en un canal privado autorizado.
8. Ingresar con el estudiante recién creado, completar un nivel, salir y volver a entrar. Verificar que su progreso se conserva.
9. Volver a la cuenta docente: revisar sus estadísticas, y confirmar que otro docente no puede consultar ni restablecer el acceso de alumnos ajenos.

## Compatibilidad con cuentas de prueba anteriores
La cuenta Docente creada por el inicializador del proyecto puede generar su perfil de grupo cuando accede a `Mis alumnos`, únicamente en Development y si su correo coincide exactamente con `PostgresBootstrap:Accounts:Docente:Email` en User Secrets. Se crea en Primaria para no alterar su perfil original. Los docentes nuevos seleccionan su nivel al darse de alta. Para probar secundaria y preparatoria, crear cuentas docentes nuevas mediante invitación.

## Seguridad, limitaciones y pendientes
- La cuenta Docente se crea únicamente si se conoce el secreto de invitación. **Este flujo de invitaciones compartidas es un MVP**, no sustituye verificación de identidad profesional, aprobación de una institución, confirmación de correo ni invitaciones individuales de un solo uso. No habilitar libremente en producción.
- Datos de menores: usar datos ficticios en desarrollo; para uso real se requiere consentimiento y marco institucional de privacidad. No hay autoservicio de recuperación de contraseña, ni obligación de cambiar contraseña en primer acceso; son pendientes.
- Las matrículas nuevas se almacenan en **`nexo.mvp_docentes`** y **`nexo.mvp_alumnos_docente`** y en Identity. La integración con las tablas curriculares finales (`nexo.estudiantes`, `nexo.grupos`, `nexo.inscripciones`, etc.) requerirá revisar el esquema original antes de migrar datos.
- El rol Docente y Alumno deben existir en Identity, como se configuró en etapas anteriores.
- La creación de cuentas, los hashes y las asignaciones de roles se hacen con ASP.NET Core Identity. Las contraseñas no se guardan en texto plano en PostgreSQL.
- Esta entrega modifica solo el código y agrega **2 tablas**; no ejecuta la migración en ninguna laptop ni prueba el arranque .NET local. Probar en Visual Studio antes de integrar a master.
