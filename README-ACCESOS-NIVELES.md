# Accesos temporales NEXO EDU — Tres niveles

Todos estos usuarios son **exclusivamente de demostración** y están definidos en `appsettings.Development.json`. No se necesita PostgreSQL. Las contraseñas son públicas dentro del repositorio privado y **no deben reutilizarse con alumnos reales**.

| Perfil | Correo | Contraseña de demostración | Vista |
|---|---|---|---|
| Alumno primaria | `alumno@nexoedu.test` | `q2ytkuHgsYjBidh!@` | Primaria · Leo |
| Alumno secundaria | `secundaria@nexoedu.test` | `K7!CieloAzul#26` | Secundaria · Sofía |
| Alumno preparatoria | `prepa@nexoedu.test` | `R9!MetaFuturo#26` | Preparatoria · Alex |
| Docente | `docente@nexoedu.test` | `MSUnINCtKnhn86C!@` | Panel docente |
| Familia | `familia@nexoedu.test` | `5cCFSQhgLWsySIk!#` | Panel familiar |

## Comenzar
1. En Visual Studio 2022 abre `NEXO-EDU.sln`, cambia a rama `feature/base-postgresql-nexo`, ejecuta Git Pull y recompila.
2. Ejecuta con el perfil `https` en entorno `Development`.
3. Pulsa **Iniciar sesión**, selecciona la tarjeta de nivel, introduce su contraseña y entra.
4. La cuenta de primaria abre la interfaz lila de primaria, secundaria la de misiones y prepa la de metas académicas.
5. Cada usuario estudiante tiene **nivel fijado por su cookie**: escribir `?nivel=preparatoria` no cambiará la cuenta de primaria.
6. Termina una actividad para ganar XP y consulta NEXO PET. Los resultados se mantienen **solo durante la sesión de ese navegador** y se limpian al cerrar sesión.

## Qué funciona ahora
- Tres accesos de alumno diferenciados + docente y familia, con autorización por roles.
- Cada nivel tiene 6 ejercicios propios; corrección y explicaciones en servidor.
- Acumulación de XP, número de actividades, porcentaje y ruta de estudio sugerida en memoria de sesión.
- Evolución visual de mascota de ejemplo; aún no guarda datos entre sesiones.
- Docente y familia pueden explorar visualmente primaria/secundaria/preparatoria con datos simulados.

## Qué sigue faltando
- Registro institucional y asignación real de cursos/grupos con PostgreSQL.
- Herramientas reales de creación de actividades, mensajes y análisis de alertas.
- Persistencia del progreso y mascotas, acceso familiar autorizado, recuperación de contraseñas, pruebas automatizadas y controles avanzados de seguridad.

## Advertencia
`DemoMode:Enabled` solo es válido en Development, nunca debe desplegarse públicamente. No recopilar información real de menores con estos usuarios. La autenticación productiva será ASP.NET Core Identity + PostgreSQL.
