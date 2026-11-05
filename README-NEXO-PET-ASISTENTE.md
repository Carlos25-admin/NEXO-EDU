# NEXO EDU · Compañero NEXO PET flotante

## Qué incorpora
- **Bienvenida individual** al entrar como alumno. Se conserva el flujo existente: si no ha nombrado a su mascota, lo dirige a NEXO PET para hacerlo; si ya tiene nombre, lo saluda en el panel.
- **Nombre propio por estudiante** persistente en PostgreSQL, usando `nexo.mvp_mascotas_nombre` (migración 006); cada alumno puede cambiarlo desde `Mi mascota`.
- Un **acompañante flotante** aparece en las páginas del alumno, biblioteca, rutas, ejercicios y resultados. Muestra el personaje y la evolución que corresponden al nivel y XP de ese estudiante.
- Mensajes de acompañamiento sencillos, adaptados a primaria (águila), secundaria (Coraje) y preparatoria (robot). En los resultados aprobados celebra y en los no aprobados recuerda que equivocarse es parte del aprendizaje.
- El acompañante aparece unos **10 segundos**, se oculta y repite la aparición cada **45 segundos** desde que se cargó la página. Hay un botón para ocultarlo durante la página actual. No se muestra en los paneles de docente o familia, ni duplica el personaje en su página NEXO PET.
- No requiere IA externa, cámaras, micrófonos, servicios de seguimiento ni claves API. Las frases son locales y predeterminadas. No se guardan conversaciones de menores.
- Respeta la preferencia de `prefers-reduced-motion` y no impide responder las preguntas cuando está oculto. Se pausa si el navegador está en segundo plano.

## Instalación
1. Respalda la base `EXO-EDU`, actualiza la rama `feature/base-postgresql-nexo` con Git Pull.
2. **Si todavía no lo hiciste, ejecuta** `NEXO-EDU/Sql/006_nombre_mascotas.sql` **en EXO-EDU**. Si ya lo ejecutaste, no hay que volver a hacerlo ni existe una migración 007 para el acompañante.
3. Comprueba:
```sql
SELECT to_regclass('nexo.mvp_mascotas_nombre') AS nombres_mascota;
```
4. Conserva el perfil `NEXO EDU - PostgreSQL`, cadena de conexión en User Secrets y `PostgresBootstrap:Enabled=false`.
5. Recompila la solución con Ctrl + Shift + B y ejecútala mediante F5.
6. Entra con un alumno de primaria. Si no había un nombre registrado, asigna uno desde la pantalla NEXO PET y vuelve al inicio.
7. Visita `Mis materias`, abre una actividad, observa cómo aparece el asistente aproximadamente 10 segundos y regresa después. Finaliza mal un ejercicio y comprueba el mensaje de ánimo. Aprueba otro y comprueba la celebración.
8. Repite en secundaria y preparatoria para comprobar personaje, etapa y nombre personalizados.
9. Cierra sesión y vuelve a entrar: debe recuperar el apodo guardado en PostgreSQL.

## Limitaciones
Este asistente es una experiencia de motivación visual **basada en reglas y mensajes predefinidos**, no un tutor de IA capaz de comprender preguntas abiertas. No diagnostica desempeño ni modifica calificaciones. El nombre usa la tabla 006 ya presente en la rama, y el XP procede de la persistencia 004. Código publicado en la rama, pendiente validación de compilación y ejecución desde Visual Studio de las laptops.
