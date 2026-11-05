# NEXO EDU · Fase 1: persistencia académica real en PostgreSQL

## Lo que cambia
La autenticación con ASP.NET Core Identity y las 48 tablas ya instaladas se conservan sin recrearlas.
Esta etapa añade **tres tablas nuevas** e independientes en el esquema `nexo`:
- `nexo.mvp_progreso`: XP, actividades, porcentaje y última recomendación por ID de usuario Identity.
- `nexo.mvp_niveles`: niveles superados por materia y XP premiado una sola vez.
- `nexo.mvp_intentos`: cada envío con resultados, fecha y respuestas seleccionadas (JSONB).

Las rutas de aprendizaje de cinco niveles, la aventura clásica y NEXO PET ya leen y escriben estos datos cuando se ejecuta el perfil **NEXO EDU - PostgreSQL**. En el perfil **NEXO EDU - Demo local**, el progreso sigue siendo temporal en Session.

## Orden seguro de instalación en CADA laptop
1. **Haz un respaldo de EXO-EDU** desde pgAdmin (Backup → formato Custom) antes de ejecutar cambios SQL.
2. Git → Pull sobre `feature/base-postgresql-nexo`.
3. En pgAdmin selecciona **EXO-EDU**, no la base `postgres` ni `Lamtra`.
4. Abre **`NEXO-EDU/Sql/004_progreso_mvp.sql`** y ejecútalo completo con F5. Es aditivo, usa `BEGIN/COMMIT`, no contiene `DROP` ni altera registros antiguos.
5. Verifica en pgAdmin con:
```sql
SELECT to_regclass('nexo.mvp_progreso') AS progreso,
       to_regclass('nexo.mvp_niveles') AS niveles,
       to_regclass('nexo.mvp_intentos') AS intentos;
```
6. En Visual Studio, deja `PostgresBootstrap:Enabled=false` y la cadena privada `Database=EXO-EDU` en User Secrets.
7. Selecciona perfil **NEXO EDU - PostgreSQL**, compila, inicia sesión con alumno primaria, abre materias y aprueba Matemáticas nivel 1. Verifica nivel 2 disponible.
8. Cierra sesión y vuelve a entrar. Debes ver el mismo avance, experiencia y etapa NEXO PET; repetir nivel 1 no debe volver a premiar XP.
9. Repite la prueba con secundaria y preparatoria para comprobar aislamiento de cada cuenta.
10. Prueba un intento fallido; debe constar en `mvp_intentos` sin desbloquear el nivel ni dar XP.

## Consultas de verificación (solo lectura)
```sql
SELECT u."Email", p.xp, p.actividades, p.ultimo_porcentaje
FROM nexo.mvp_progreso p
JOIN public."AspNetUsers" u ON u."Id"=p.usuario_id
ORDER BY u."Email";

SELECT u."Email", n.clave, n.nivel, n.mejor_porcentaje, n.xp_ganado
FROM nexo.mvp_niveles n
JOIN public."AspNetUsers" u ON u."Id"=n.usuario_id
ORDER BY u."Email",n.clave,n.nivel;

SELECT u."Email", i.materia, i.clave, i.nivel,
       i.correctas, i.total, i.porcentaje, i.aprobado, i.fecha
FROM nexo.mvp_intentos i
JOIN public."AspNetUsers" u ON u."Id"=i.usuario_id
ORDER BY i.fecha DESC LIMIT 30;
```

## Garantías y límites
- La lectura usa siempre el **ID de Identity** autenticado; ningún usuario selecciona otro ID desde URL o formulario.
- La corrección y el desbloqueo de niveles se validan en servidor.
- Las actualizaciones de XP, nivel y registros de intentos se realizan en una sola transacción. Cada nivel premia XP una única vez.
- No se guarda la contraseña del alumno ni se crea ninguna cuenta nueva.
- **El contenido pedagógico de las 192 preguntas permanece en C#**; esta fase guarda intentos y resultados, no convierte todavía el catálogo a tablas curriculares.
- Las nuevas tres tablas son exclusivas del prototipo MVP; integrar los datos con las 41 tablas académicas finales requiere revisar antes el SQL original para evitar columnas supuestas.
- La migración no corre automáticamente: debe aplicarse una vez por laptop. Si no existe, las rutas de progreso darán error de tabla inexistente hasta que se ejecute.
- Se recomienda configurar un rol PostgreSQL con mínimos privilegios antes de despliegues no locales.
- No probamos compilación ni ejecución en Windows desde este entorno; validar F5 y capturas del equipo antes de integrar a master.
