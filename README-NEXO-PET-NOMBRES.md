# NEXO EDU · Bienvenida y nombre propio para NEXO PET

## Nueva experiencia del alumno
- Cuando un alumno inicia sesión se prepara un mensaje personalizado con su nombre.
- Si todavía no bautizó a su mascota, la primera entrada lo lleva directamente a **Mi mascota** con una bienvenida y el formulario para nombrarla.
- Si ya le puso nombre, entra normalmente a su panel y recibe una bienvenida de su mascota.
- El alumno puede volver a **Mi mascota** para cambiar el nombre cuantas veces necesite, sin afectar su XP ni sus evoluciones.
- El apodo aparece tanto en el panel de estudiante como en la pantalla NEXO PET. El personaje sigue siendo el águila, Coraje o NexoBot según el nivel.
- Se admiten nombres de 2 a 24 caracteres (letras, números y espacios); el servidor valida y limita la longitud.
- Cada alumno únicamente puede cambiar el nombre de su mascota, determinada por la identidad de ASP.NET Identity autenticada. No existe un parámetro para editar cuentas ajenas.

## Instalación obligatoria de PostgreSQL
1. Hacer respaldo de **EXO-EDU**.
2. En Visual Studio ejecutar Git → Pull en `feature/base-postgresql-nexo`.
3. En pgAdmin abrir **EXO-EDU** y ejecutar **`NEXO-EDU/Sql/006_nombre_mascotas.sql`** (solo agrega `nexo.mvp_mascotas_nombre`).
4. Verificar:
```sql
SELECT to_regclass('nexo.mvp_mascotas_nombre') AS nombres_mascota;
```
5. Mantener `PostgresBootstrap:Enabled=false` y perfil `NEXO EDU - PostgreSQL`.
6. Compilar y ejecutar F5. Iniciar como alumno y comprobar que, si aún no tiene apodo, se abre la pantalla NEXO PET.
7. Elegir un apodo, guardar, regresar al panel y verificar el nombre en su tarjeta.
8. Cerrar sesión y entrar de nuevo: el nombre personalizado debe mantenerse y aparecer la bienvenida.
9. Entrar con otro alumno y verificar que su mascota conserva un nombre independiente.
10. Probar a cambiar un nombre y comprobar que XP, actividades y evolución se mantienen iguales.

## Consulta de comprobación
```sql
SELECT u."Email", m.nombre, m.actualizado_en
FROM nexo.mvp_mascotas_nombre m
JOIN public."AspNetUsers" u ON u."Id"=m.usuario_id
ORDER BY m.actualizado_en DESC;
```

## Alcance
El modo **Demo local** utiliza Session para el apodo de la mascota, por lo que es temporal; el perfil PostgreSQL lo conserva de manera permanente. Esta fase no renombra cuentas de alumno, no toca la tabla de progreso, no modifica las evoluciones y no borra datos.

**No ejecutar F5 sobre PostgreSQL con esta versión antes de aplicar 006**, porque las consultas del nombre requieren la nueva tabla.

La compilación y la prueba end-to-end se validan en Visual Studio del equipo; aquí solo se suben los archivos al repositorio.
