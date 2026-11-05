# NEXO EDU — paneles por nivel (fase de interfaz)

## Nivel primaria, secundaria y preparatoria
A partir de las referencias visuales aportadas por el equipo, hay **tres experiencias educativas** con el mismo acceso por rol.

| Nivel | Alumno | Docente | Familia |
|---|---|---|---|
| Primaria | Aventuras cortas, colores lila/amarillo, mascota cercana | Aula simple, avances y apoyos | Resumen semanal, actividad en casa, mensaje de ejemplo |
| Secundaria | Misiones, metas y mascota | Desempeño por concepto y alertas | Acompañamiento y progreso |
| Preparatoria | Competencias, objetivos, autonomía | Indicadores académicos y seguimiento | Acompañamiento respetuoso de autonomía |

### Probar desde Visual Studio
1. En la rama `feature/base-postgresql-nexo`, hacer Git Pull y recompilar.
2. Ejecutar HTTPS en entorno Development. El modo demo funciona sin PostgreSQL.
3. Entrar usando las tres cuentas de prueba ya documentadas en `README-LOGIN-POSTGRES.md`.
4. Una vez iniciado, cada perfil ofrece el selector **Primaria / Secundaria / Prepa**. Es un selector de **vistas de demostración**, NO modifica permisos ni el nivel escolar real del usuario.
5. En el perfil alumno, pulsar el botón de actividad; cada nivel tiene tres ejercicios adaptados a la etapa. Las respuestas se corrigen en el servidor, pero no se guardan todavía.

### Pendientes
- Cuando exista PostgreSQL, vincular nivel, grupo, alumno/docente/familiar con entidades institucionales y permisos verificables; **no confiar en el selector de URL** para determinar niveles reales.
- Cargar contenidos curriculares revisados, progreso persistente, intervenciones reales, autorización parental, mensajería y NEXO PET persistente.
- Los indicadores, mensajes y nombres son ficticios y no se deben mostrar como datos reales.
- Esta actualización no reemplaza la ruta de diagnóstico por conceptos existente de secundaria; añade actividades por nivel.

### Seguridad
La autenticación sigue protegida por roles. El selector de nivel **no permite** ver otro tipo de perfil. Las cuentas del modo local son de demostración, no de producción. Nunca desplegar la configuración Development con contraseñas de prueba.
