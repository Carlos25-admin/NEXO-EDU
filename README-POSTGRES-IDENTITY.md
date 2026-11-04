# NEXO EDU · Activar acceso con Identity y PostgreSQL (5 cuentas de prueba)

## IMPORTANTE
El script SQL ya creó las siete tablas `public."AspNet*"` y el esquema `nexo`. No crear otra base ni volver a ejecutar los archivos 001/002 si ya terminaron correctamente.

El backend ahora usa `NexoIdentityDbContext` exclusivamente para Identity; no ejecuta `EnsureCreated` ni `Migrate` sobre la base SQL existente.

## Credenciales privadas locales
En Visual Studio: clic derecho en el proyecto NEXO-EDU → **Administrar secretos de usuario**. Agregar la siguiente estructura con contraseñas nuevas, diferentes y fuertes (12+ caracteres, mayúsculas, minúsculas, número y símbolo):

```json
{
  "ConnectionStrings": {
    "NexoEdu": "Host=127.0.0.1;Port=5432;Database=EXO-EDU;Username=USUARIO_POSTGRES;Password=CLAVE_POSTGRES"
  },
  "PostgresBootstrap": {
    "Enabled": true,
    "Accounts": {
      "Primaria": {"Email":"primaria.db@nexoedu.test","Password":"REEMPLAZAR_CON_CLAVE_PRIVADA_1"},
      "Secundaria": {"Email":"secundaria.db@nexoedu.test","Password":"REEMPLAZAR_CON_CLAVE_PRIVADA_2"},
      "Preparatoria": {"Email":"prepa.db@nexoedu.test","Password":"REEMPLAZAR_CON_CLAVE_PRIVADA_3"},
      "Docente": {"Email":"docente.db@nexoedu.test","Password":"REEMPLAZAR_CON_CLAVE_PRIVADA_4"},
      "Familia": {"Email":"familia.db@nexoedu.test","Password":"REEMPLAZAR_CON_CLAVE_PRIVADA_5"}
    }
  }
}
```

**Nunca publiques User Secrets ni la contraseña real de postgres.** Cada laptop tiene su propia configuración y base local, por eso cada compañero debe realizar este paso en su equipo. En la versión de despliegue conviene usar un usuario PostgreSQL dedicado con permisos limitados, no `postgres`.

## Activar el perfil correcto
1. Git → Pull en `feature/base-postgresql-nexo`.
2. Cierra Visual Studio y vuelve a abrir la solución si no aparecen los perfiles nuevos.
3. Elige **NEXO EDU - PostgreSQL** junto al botón verde (NO `NEXO EDU - Demo local`).
4. Compila y ejecuta. Solo en Development, el inicializador usa UserManager para crear roles, hashes y perfiles sin modificar usuarios existentes.
5. Después de que arranque correctamente, cambia `PostgresBootstrap:Enabled` a `false`. Las cuentas ya quedan en PostgreSQL y pueden seguir iniciando sesión.
6. Abre `/Cuenta/Ingresar` e introduce el correo y contraseña que definiste en User Secrets.

## Verificación pgAdmin
```sql
SELECT u."Email", r."Name" AS rol
FROM public."AspNetUsers" u
JOIN public."AspNetUserRoles" ur ON ur."UserId" = u."Id"
JOIN public."AspNetRoles" r ON r."Id" = ur."RoleId"
ORDER BY u."Email";
```
No consultar `"PasswordHash"` para recuperar contraseñas; no es reversible.

## Notas
- Si la contraseña ya existe y ejecutas de nuevo el inicializador, **no la cambia**.
- El botón para seleccionar alumno/docente/familia solo precarga correos de la versión demo local. En modo PostgreSQL se usan los correos privados configurados.
- Los paneles y actividades todavía son prototipos; la experiencia XP que se presenta sigue almacenada temporalmente en sesión. Los nuevos perfiles académicos se almacenan en PostgreSQL, pero la persistencia de intentos y XP requiere su propia integración.
- PostgreSQL debe usar tablas `AspNet*` con la estructura exacta de los scripts anteriores.
- Las cuentas de prueba NO son cuentas para usuarios reales ni menores.
