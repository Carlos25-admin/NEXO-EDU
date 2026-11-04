# NEXO EDU · Cinco niveles por módulo educativo

## Alcance
- Primaria: Matemáticas, Español, Ciencias y Convivencia, cinco niveles cada una.
- Secundaria: Matemáticas, Español, Ciencias y Ciudadanía digital, cinco niveles cada una.
- Preparatoria: Matemáticas, Comunicación, Ciencias y Proyecto de vida, cinco niveles cada una.
- **12 materias × 5 niveles = 60 misiones**. El banco anterior tenía 48 preguntas (4 por materia en nivel 1); se agregan **144 nuevas** (3 en cada nivel 2–5), **192 en total**.
- Los cinco niveles tienen ejercicios diferentes en cada materia, con opciones, una respuesta válida y explicación después del envío.

## Reglas de progresión
1. Nivel 1 se abre al entrar a la materia.
2. Con **65% o más** de aciertos se supera el nivel y se desbloquea el siguiente.
3. En nivel 1 hacen falta **3/4** aciertos; en niveles 2-5 hacen falta **2/3**.
4. El alumno puede repetir sin límite, pero **el XP se otorga solo al superar el nivel por primera vez**, no por repetirlo; no se restan puntos por errores.
5. Cada materia tiene un seguimiento independiente de avance y desbloqueos. El servidor valida el nivel accesible tanto al abrir el ejercicio como al corregir el formulario.
6. La evolución de NEXO PET utiliza el XP total de las actividades durante la sesión, como antes.

## Rutas
- `/Modulos/Biblioteca`: todas las materias con estado de niveles.
- `/Modulos/Ruta?clave=numeros`: mapa de las cinco etapas.
- `/Modulos/Resolver?clave=numeros&nivel=1`: ejercicio.
- La acción `Calificar` verifica nivel y respuestas en servidor con CSRF.

## Revisión en Visual Studio
1. Actualizar rama `feature/base-postgresql-nexo` con Git Pull y compilar.
2. Dejar `PostgresBootstrap:Enabled=false` (las cuentas ya existen).
3. Entrar con Primaria, abrir Matemáticas y comprobar que nivel 2 aparece bloqueado inicialmente.
4. Resolver al menos 3 de 4 correctamente y confirmar que se desbloquea el nivel 2.
5. En el nivel 2 responder al menos 2 de 3 correctamente, y avanzar.
6. Repetir un nivel aprobado: el XP debe permanecer igual.
7. Probar otro módulo: debe iniciar independientemente desde nivel 1.
8. Intentar navegar directamente a un nivel aún bloqueado cambiando el parámetro URL: la aplicación debe devolver a la ruta.
9. Cerrar sesión y volver a entrar; como el almacenamiento es temporal, el progreso puede perderse al finalizar la sesión.
10. Repetir con Secundaria y Preparatoria, verificar que los bancos educativos cambian por nivel.

## Límites
**Este es un prototipo educativo funcional**, no una plataforma académica certificada. Los 192 ejercicios constituyen ejemplos iniciales y deberán someterse a revisión pedagógica por grado y programa curricular antes de usarse con estudiantes reales. El seguimiento de niveles y XP todavía se conserva en la sesión web, NO en PostgreSQL. No se han realizado migraciones ni modificaciones de las 48 tablas. La fase posterior es persistencia de intentos, calificaciones, experiencia y evolución NEXO PET, con autorizaciones por identidad real.
