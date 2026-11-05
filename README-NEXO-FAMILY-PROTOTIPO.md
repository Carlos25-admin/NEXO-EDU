# NEXO FAMILY: guía de prueba de la fase 007

El panel permite, únicamente en Development, vincular un alumno ficticio registrado por su docente mediante coincidencia exacta de nombre completo, nivel y grado. Después muestra XP, actividad reciente, niveles y mascota con consultas PostgreSQL asociadas al usuario del rol Familia que ha iniciado sesión.

**Esta coincidencia no acredita parentesco.** La búsqueda y visualización están deshabilitadas fuera de Development. No utilizar información de menores reales. Antes de publicar la plataforma, reemplazar por un flujo de autorización institucional y verificación de responsables.

Instalación local:
1. Crear una copia de seguridad de EXO-EDU.
2. Descargar los últimos cambios de feature/base-postgresql-nexo.
3. Confirmar que se aplicaron las migraciones 004, 005 y 006.
4. Ejecutar `NEXO-EDU/Sql/007_vinculos_familiares_desarrollo.sql` sobre EXO-EDU.
5. Confirmar con `SELECT to_regclass('nexo.mvp_vinculos_familiares');`.
6. Mantener PostgresBootstrap:Enabled=false y NEXO EDU - PostgreSQL en Development.
7. Compilar y probar una cuenta Familia y una matrícula ficticia.
8. Buscar por nombre completo, nivel y grado; confirmar vínculo y estadísticas reales.
9. Probar búsquedas incorrectas, coincidencias múltiples y acceso de un familiar a datos sin vínculo.

La pantalla no altera calificaciones ni el progreso. El equipo debe verificar compilación y funcionamiento en Visual Studio antes de integrar a master.
