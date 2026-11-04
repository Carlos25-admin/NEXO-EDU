# NEXO EDU · Biblioteca educativa por niveles

## Qué incluye esta iteración
- Primaria: 4 materias / 16 preguntas: matemáticas, español, ciencias, convivencia.
- Secundaria: 4 materias / 16 preguntas: matemáticas, español, ciencias, ciudadanía digital.
- Preparatoria: 4 materias / 16 preguntas: matemáticas, comunicación, ciencias, proyecto de vida.
- Total **12 actividades y 48 preguntas de opción múltiple** con retroalimentación individual después de enviar.
- Nueva biblioteca de materias con variantes visuales por nivel.
- Nuevos accesos en panel de alumno y navegación lateral.
- Espacio docente de planeación curricular y guía familiar con consejos por nivel.
- Rutas protegidas por rol; el alumno no cambia de nivel introduciendo otra URL.
- XP y actividad completada reflejados en progreso de sesión del alumno, sin alterar las tablas PostgreSQL.

## Rutas
- `/Modulos/Biblioteca` (Alumno)
- `/Modulos/Resolver?clave=lectura` (Alumno primaria, por ejemplo)
- `/Modulos/RecursosDocente?nivel=secundaria` (Docente)
- `/Modulos/GuiaFamiliar?nivel=preparatoria` (Familia)

## Verificación manual para cada laptop
1. Git → Pull en `feature/base-postgresql-nexo`; seleccionar perfil `NEXO EDU - PostgreSQL`.
2. Dejar `PostgresBootstrap:Enabled=false`, ya que las cinco cuentas están creadas.
3. Compilar y ejecutar F5; autenticar Alumno primaria. Abrir «Explorar materias».
4. Resolver los cuatro ejercicios de Español; comprobar retroalimentación y XP; regresar a la biblioteca.
5. Entrar con alumno secundaria y prepa y comprobar que no pueden cargar el catálogo de otro nivel cambiando parámetros.
6. Entrar con Docente y Familia; comprobar que cada rol ve sus recursos correspondientes y no las pantallas del otro rol.
7. Confirmar que el login PostgreSQL sigue funcionando y que los datos originales están intactos.

## Alcance y próximos pasos
Los ejercicios y el catálogo son funcionales como demostración, pero **sus resultados y XP aún viven en Session y no en PostgreSQL**. El panel docente y la guía familiar muestran contenidos de ejemplo, no datos personales de alumnos. La creación/edición de contenidos por docente, asignaciones a grupos, expedientes, vínculos familiares autorizados, NEXO ALERT, mensajería y evolución persistente de NEXO PET siguen pendientes de integración.

No ejecutar `EnsureCreated` ni recrear las 48 tablas sobre la base existente.
