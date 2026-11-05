# NEXO EDU — accesos de demostración listos

Al ejecutar con el perfil **https** de Visual Studio 2022 en **Development**, la aplicación muestra la página NEXO EDU, con inicio de sesión en `/Cuenta/Ingresar`. **No se necesita PostgreSQL, migraciones ni modificar User Secrets** para probar los perfiles.

| Perfil | Correo | Contraseña exclusivamente de prueba |
|---|---|---|
| Alumno | alumno@nexoedu.test | q2ytkuHgsYjBidh!@ |
| Docente | docente@nexoedu.test | MSUnINCtKnhn86C!@ |
| Familia | familia@nexoedu.test | 5cCFSQhgLWsySIk!# |

Estas credenciales de prueba están intencionalmente publicadas en `appsettings.Development.json`, **NO son secretas**, y solo funcionan si `ASPNETCORE_ENVIRONMENT=Development` y `DemoMode:Enabled=true`. No se permite registro de nuevas cuentas ni datos reales en el modo de prueba. El alumnado y las familias ven información ficticia; no hay almacenamiento persistente ni bloqueo de intentos en esta modalidad.

## Cómo probar
1. Seleccionar la rama `feature/base-postgresql-nexo`, extraer cambios desde Visual Studio y recompilar.
2. Ejecutar con perfil `https` y entrar a **Iniciar sesión**.
3. Probar una cuenta por vez. Cada una solo accede al módulo autorizado.
4. Usar **Cerrar sesión** para cambiar entre Alumno, Docente y Familia.

## Si tienes secretos anteriores
Aunque exista una cadena `ConnectionStrings:NexoEdu` en `secrets.json`, el modo de demostración **no se conectará** a PostgreSQL. Sin embargo, un `DemoMode:Enabled=false` en User Secrets o en variables de entorno tiene prioridad sobre el archivo Development. Elimina esa sobrescritura si quieres seguir sin base de datos. Los valores de `DemoUsers` también pueden ser sobrescritos por User Secrets: en ese caso usa tus claves locales.

## Para pasar a PostgreSQL
En una fase posterior, desactiva `DemoMode:Enabled`, configura la conexión de PostgreSQL por User Secrets y utiliza Identity. El inicializador existente se limita a una base nueva y vacía en Development, y no sustituye migraciones EF Core. **Nunca habilites el modo de demostración en un servidor público ni reutilices estas contraseñas para cuentas reales.**
