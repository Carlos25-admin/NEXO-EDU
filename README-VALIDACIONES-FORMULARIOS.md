# Validaciones integrales — NEXO EDU

## Reglas aplicadas
| Campo | Regla |
|---|---|
| Nombre de docente, alumno, familiar buscado y mascota | Solo letras (incluye español, ñ, tildes), espacios, guion o apóstrofe entre palabras. Sin dígitos. Mínimo y máximo según campo. |
| Institución educativa | Letras, números y signos habituales de nombres oficiales (p. ej. Secundaria Técnica 25). |
| Grado | Se selecciona del catálogo numérico (1–6 en primaria, 1–3 en secundaria/prepa); se vuelve a validar en servidor. |
| Grupo | 1–12 letras o números, sin espacios ni símbolos. |
| Usuario manual | 3–30 caracteres, letras minúsculas y números; inicia con letra; comprobación de duplicados en Identity. |
| Contraseña manual del alumno | 12–64 caracteres, al menos una mayúscula, minúscula y número. El servidor respeta el modo automático. |
| Contraseña del docente | 12–128 caracteres como mínimo de formulario; además se exige la política de seguridad de Identity. No se restringen los símbolos. |
| Correo | Formato email y longitud máxima. |
| Nombre de mascota | 2–24 caracteres y letras, con acentos/espacios/guiones/apóstrofes. |
| Código docente | Campo obligatorio y comparación con secreto privado; longitud mínima de configuración. |
| Respuestas de ejercicios | Se siguen aceptando únicamente las opciones válidas para cada pregunta, en el servidor. |

## Doble validación
- **Interfaz**: ayudas en tiempo real y mensajes accesibles, sin borrar caracteres automáticamente. Campos obligatorios y límites HTML.
- **Servidor**: DataAnnotations, reglas centralizadas en `Models/ReglasFormulario.cs`, validación condicional de usuario/clave manual, rangos en controladores y reglas preexistentes de Identity.
- No se ejecuta SQL de migración ni se actualizan registros antiguos. Los nombres previamente almacenados pueden contener dígitos: solo las altas y modificaciones nuevas usan las reglas revisadas.

## Verificación en Visual Studio
1. Git Pull de `feature/base-postgresql-nexo`; recompilar (Ctrl+Shift+B); ejecutar perfil PostgreSQL.
2. Registro docente: escribir números en nombre (debe rechazarse), usar letras acentuadas (debe aceptarse), institución «Secundaria Técnica 25» (válida), correo sin @ (rechazado), claves distintas (rechazado).
3. Alta alumno: «Carlos123» (rechazado), «María José» (válido), grupo «2B» (válido), «A@» (rechazado), usuario manual `carlos4821` (válido), `carlos 123` (rechazado).
4. Contraseña manual: valores cortos/sin mezcla de tipos (rechazados); 12+ con mayúscula, minúscula y dígito (válido).
5. Familia: búsqueda con nombres alfabéticos y grado según el nivel; no deben vincularse alumnos si faltan datos.
6. NEXO PET: nombres válidos en español y rechazo de números/símbolos.
7. Con navegador: desactivar JavaScript o alterar el formulario. El servidor debe conservar las mismas reglas.
8. Confirmar que cuentas y progresos existentes siguen funcionando.

**Pendiente:** pruebas de compilación y ejecución en Visual Studio de los integrantes; no disponemos del SDK de .NET en este entorno.
