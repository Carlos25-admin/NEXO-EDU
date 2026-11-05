# NEXO PET · Mascotas evolutivas por nivel
Se integraron al módulo real 3 compañeros ilustrados como vectores SVG, cada uno con 3 variantes de diseño (9 artes en total) y animación fluida.

| Nivel | Mascota | Etapa 1 (0 XP) | Etapa 2 (70 XP) | Etapa 3 (180 XP) |
|---|---|---|---|---|
| Primaria | Nexito, el águila | Nexito Mini | Nexito Explorador | Nexito Guardián |
| Secundaria | Coraje, perrito rosa | Cachorro curioso | Compañero decidido | Héroe del aprendizaje |
| Preparatoria | NexoBot | NexoBot Jr. | NexoBot V2 | NexoBot Pro |

**Animaciones**: respiración/flotación, brillo ambiental, alas móviles, orejas y cola, órbita del robot; botones para saludo, salto y celebración. El modo de movimiento reducido del dispositivo se respeta mediante prefers-reduced-motion.

**Rutas y uso:** Como alumno de cada nivel, abrir `/NexoDemo/Mascota`. En `/NexoDemo/Estudiante` aparece una versión compacta de su mascota. Se determina el nivel mediante el claim de la cuenta (no mediante parámetros URL) y la etapa mediante XP del prototipo. Los botones ejecutan animaciones en el navegador y muestran frases por mascota.

**Seguridad**: No se han modificado los scripts SQL ni la autenticación. **No son modelos 3D descargables ni rigs esqueléticos**: son ilustraciones vectoriales con efectos 2.5D, animaciones CSS/SVG y controles JavaScript. La evolución no se guarda aún en PostgreSQL: depende del XP de sesión.

**Comprobación**: hacer Git Pull en `feature/base-postgresql-nexo`, dejar PostgresBootstrap desactivado, compilar y probar el acceso de los tres alumnos. Abrir el apartado NEXO PET y probar controles, evolución de etapa al resolver ejercicios y la vista móvil. Si usas un navegador con bloqueo de animación, comprueba el ajuste de movimiento reducido.

**Nota sobre diseño:** Las ilustraciones vectoriales reproducen los rasgos principales de los diseños acordados, pero no incluyen archivos originales 3D ni imágenes fotorrealistas.
