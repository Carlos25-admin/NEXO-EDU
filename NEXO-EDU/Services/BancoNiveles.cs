using System.Collections.Generic;

namespace NEXO_EDU.Services;

// Cinco niveles por materia. El nivel 1 procede de CatalogoLecciones.Todas.
public sealed record NivelPreguntas(int Nivel, IReadOnlyList<PreguntaNexo> Preguntas);
public static class BancoNiveles
{
    private static readonly IReadOnlyDictionary<string, NivelPreguntas[]> Adicionales =
        new Dictionary<string, NivelPreguntas[]>(StringComparer.OrdinalIgnoreCase)
    {
        ["numeros"] = new[]
        {
            new NivelPreguntas(2, new[]
            {
                new PreguntaNexo(1, "¿Cuánto es 14 + 9?", new[] { "23", "24", "22" }, 0, "Catorce más nueve son veintitrés."),
                new PreguntaNexo(2, "¿Cuánto es 30 − 12?", new[] { "20", "18", "16" }, 1, "Treinta menos doce son dieciocho."),
                new PreguntaNexo(3, "¿Cuál número es par?", new[] { "17", "23", "28" }, 2, "Veintiocho es divisible entre dos.")
            }),
            new NivelPreguntas(3, new[]
            {
                new PreguntaNexo(1, "Tres cajas tienen 6 galletas cada una. ¿Cuántas hay?", new[] { "18", "9", "12" }, 0, "3 × 6 = 18."),
                new PreguntaNexo(2, "¿Cuánto es 24 ÷ 4?", new[] { "8", "6", "5" }, 1, "Cuatro grupos iguales de seis hacen veinticuatro."),
                new PreguntaNexo(3, "¿Cuánto es 7 × 5?", new[] { "30", "40", "35" }, 2, "Siete grupos de cinco suman treinta y cinco.")
            }),
            new NivelPreguntas(4, new[]
            {
                new PreguntaNexo(1, "¿Cuál es la mitad de 18?", new[] { "9", "6", "12" }, 0, "18 dividido entre 2 es 9."),
                new PreguntaNexo(2, "¿Qué fracción representa una de cuatro partes?", new[] { "1/2", "1/4", "4/1" }, 1, "Una parte entre cuatro es un cuarto."),
                new PreguntaNexo(3, "¿Cuál es el perímetro de un cuadrado de lado 3 cm?", new[] { "6 cm", "9 cm", "12 cm" }, 2, "Cuatro lados de tres centímetros suman doce.")
            }),
            new NivelPreguntas(5, new[]
            {
                new PreguntaNexo(1, "Una mochila cuesta $180 y descuentan $30. ¿Cuánto pagas?", new[] { "$150", "$160", "$210" }, 0, "180 − 30 = 150."),
                new PreguntaNexo(2, "Si recorres 250 m y después 350 m, ¿cuántos metros avanzaste?", new[] { "500 m", "600 m", "700 m" }, 1, "250 + 350 = 600."),
                new PreguntaNexo(3, "Una receta requiere 2 vasos por pastel. Para 4 pasteles necesitas...", new[] { "6 vasos", "7 vasos", "8 vasos" }, 2, "Dos vasos por cuatro pasteles son ocho.")
            })
        },
        ["lectura"] = new[]
        {
            new NivelPreguntas(2, new[]
            {
                new PreguntaNexo(1, "En «Luis corre rápido», ¿cuál palabra indica una acción?", new[] { "Luis", "corre", "rápido" }, 1, "El verbo correr expresa la acción."),
                new PreguntaNexo(2, "¿Qué signo termina una pregunta?", new[] { "¿", "¡", "?" }, 2, "El signo de cierre de interrogación es ?."),
                new PreguntaNexo(3, "¿Cuál palabra es plural?", new[] { "Árboles", "Árbol", "Hoja" }, 0, "Árboles nombra a más de uno.")
            }),
            new NivelPreguntas(3, new[]
            {
                new PreguntaNexo(1, "¿Cuál oración está escrita correctamente?", new[] { "Ayer fuí al parque.", "Ayer fui al parque.", "Ayer fui al Parque." }, 1, "La forma fui no lleva tilde."),
                new PreguntaNexo(2, "En «El perro fiel duerme», «fiel» es...", new[] { "Verbo", "Sustantivo", "Adjetivo" }, 2, "Fiel describe al perro."),
                new PreguntaNexo(3, "¿Qué palabra indica orden temporal?", new[] { "Después", "Azul", "Pequeño" }, 0, "Después sitúa un hecho en el tiempo.")
            }),
            new NivelPreguntas(4, new[]
            {
                new PreguntaNexo(1, "«María llevó paraguas porque llovía». ¿Por qué lo llevó?", new[] { "Por el sol", "Porque llovía", "Para leer" }, 1, "La oración explica la causa: lluvia."),
                new PreguntaNexo(2, "¿Cuál es la idea principal de un texto?", new[] { "Un dato aislado", "La última palabra", "El mensaje más importante" }, 2, "La idea principal reúne el sentido central."),
                new PreguntaNexo(3, "¿Cuál oración expresa una comparación?", new[] { "Corre como el viento", "Ayer cociné", "La casa está abierta" }, 0, "«Como» establece una comparación.")
            }),
            new NivelPreguntas(5, new[]
            {
                new PreguntaNexo(1, "¿Para qué sirve un título informativo?", new[] { "Para ocultar el tema", "Para anticipar el tema", "Para cambiar la historia" }, 1, "Un título informa de qué trata el texto."),
                new PreguntaNexo(2, "¿Qué fuente ayuda a comprobar una noticia escolar?", new[] { "Un rumor", "Una cadena anónima", "El comunicado oficial" }, 2, "Una fuente oficial permite verificar datos."),
                new PreguntaNexo(3, "En un cuento, el conflicto es...", new[] { "El problema que afrontan los personajes", "La fecha de publicación", "El nombre del autor" }, 0, "El conflicto pone en marcha la historia.")
            })
        },
        ["naturaleza"] = new[]
        {
            new NivelPreguntas(2, new[]
            {
                new PreguntaNexo(1, "¿Qué parte de la planta absorbe agua del suelo?", new[] { "Flor", "Raíz", "Fruto" }, 1, "Las raíces absorben agua y nutrientes."),
                new PreguntaNexo(2, "¿Qué animal es mamífero?", new[] { "Mariposa", "Rana", "Delfín" }, 2, "Los delfines son mamíferos."),
                new PreguntaNexo(3, "¿Qué sucede al calentar hielo?", new[] { "Se derrite", "Se convierte en roca", "Desaparece sin cambio" }, 0, "El hielo pasa a agua líquida.")
            }),
            new NivelPreguntas(3, new[]
            {
                new PreguntaNexo(1, "¿Qué astro ilumina la Tierra durante el día?", new[] { "La Luna", "El Sol", "Marte" }, 1, "El Sol proporciona la luz del día."),
                new PreguntaNexo(2, "¿Cómo llamamos al agua que cae de las nubes?", new[] { "Viento", "Niebla", "Lluvia" }, 2, "La lluvia es precipitación líquida."),
                new PreguntaNexo(3, "¿Cuál órgano usamos para respirar?", new[] { "Pulmones", "Estómago", "Huesos" }, 0, "Los pulmones intercambian gases al respirar.")
            }),
            new NivelPreguntas(4, new[]
            {
                new PreguntaNexo(1, "¿Quién produce su alimento mediante fotosíntesis?", new[] { "Un gato", "Una planta verde", "Una roca" }, 1, "Las plantas verdes aprovechan la luz solar."),
                new PreguntaNexo(2, "¿Qué material suele ser atraído por un imán?", new[] { "Madera", "Plástico", "Hierro" }, 2, "El hierro presenta propiedades magnéticas."),
                new PreguntaNexo(3, "¿Qué movimiento de la Tierra produce el día y la noche?", new[] { "Rotación", "Traslación", "Evaporación" }, 0, "La rotación alterna la exposición al Sol.")
            }),
            new NivelPreguntas(5, new[]
            {
                new PreguntaNexo(1, "Si mezclas arena y agua, ¿cómo puedes separarlas?", new[] { "Con un imán", "Por filtración", "Calentando hasta quemar" }, 1, "Un filtro retiene la arena."),
                new PreguntaNexo(2, "¿Qué acción ayuda a ahorrar agua?", new[] { "Dejar correr el grifo", "Lavar con manguera abierta", "Cerrar el grifo al cepillarse" }, 2, "Cerrar el grifo evita desperdicio."),
                new PreguntaNexo(3, "¿Qué secuencia forma parte del ciclo del agua?", new[] { "Evaporación, condensación, precipitación", "Congelación, combustión, erosión", "Rotación, eclipse, traslación" }, 0, "El agua puede evaporarse, condensarse y precipitar.")
            })
        },
        ["convivencia"] = new[]
        {
            new NivelPreguntas(2, new[]
            {
                new PreguntaNexo(1, "¿Qué haces cuando no entiendes una instrucción?", new[] { "Te burlas", "Pides que te la expliquen", "Abandonas siempre" }, 1, "Preguntar es una forma responsable de aprender."),
                new PreguntaNexo(2, "Al jugar en equipo, lo adecuado es...", new[] { "Imponer tus reglas", "No escuchar", "Respetar los acuerdos" }, 2, "Los acuerdos ayudan a convivir."),
                new PreguntaNexo(3, "¿Cómo muestras gratitud?", new[] { "Dando las gracias", "Insultando", "Quitando materiales" }, 0, "Agradecer reconoce el apoyo de otros.")
            }),
            new NivelPreguntas(3, new[]
            {
                new PreguntaNexo(1, "Si alguien piensa diferente, conviene...", new[] { "Interrumpirlo", "Escucharlo con respeto", "Burlarte" }, 1, "Podemos escuchar sin estar de acuerdo."),
                new PreguntaNexo(2, "Ante un conflicto, la mejor primera acción es...", new[] { "Gritar", "Golpear", "Hablar con calma" }, 2, "Dialogar permite buscar soluciones."),
                new PreguntaNexo(3, "¿Qué significa colaborar?", new[] { "Trabajar juntos por una meta", "Dejar todo a otra persona", "Competir por todo" }, 0, "Colaborar implica aportar y apoyar.")
            }),
            new NivelPreguntas(4, new[]
            {
                new PreguntaNexo(1, "¿Qué es ponerse en el lugar de otra persona?", new[] { "Ignorarla", "Practicar empatía", "Copiar sus acciones" }, 1, "La empatía busca comprender sus sentimientos."),
                new PreguntaNexo(2, "¿Qué conviene hacer ante una burla repetida?", new[] { "Repetirla", "Guardarla en secreto", "Pedir apoyo a un adulto de confianza" }, 2, "Pedir apoyo ayuda a detener el maltrato."),
                new PreguntaNexo(3, "Para incluir a un nuevo compañero podemos...", new[] { "Invitarlo a participar", "No dirigirle la palabra", "Excluirlo del grupo" }, 0, "Invitarlo facilita su integración.")
            }),
            new NivelPreguntas(5, new[]
            {
                new PreguntaNexo(1, "En un proyecto compartido, ¿qué ayuda al grupo?", new[] { "Cambiar reglas sin avisar", "Repartir tareas con claridad", "No cumplir acuerdos" }, 1, "Distribuir tareas facilita colaborar."),
                new PreguntaNexo(2, "Si cometiste un daño sin querer, lo adecuado es...", new[] { "Reírte", "Negarlo", "Disculparte y reparar" }, 2, "Asumir y reparar el daño fortalece la convivencia."),
                new PreguntaNexo(3, "Si ves que excluyen a alguien, puedes...", new[] { "Invitarlo y buscar apoyo si hace falta", "Unirte a la exclusión", "Difundir rumores" }, 0, "La inclusión y el apoyo protegen a todos.")
            })
        },
        ["fracciones"] = new[]
        {
            new NivelPreguntas(2, new[]
            {
                new PreguntaNexo(1, "¿Cuánto es 2/5 + 1/5?", new[] { "3/10", "3/5", "1/5" }, 1, "Sumamos numeradores y conservamos denominador."),
                new PreguntaNexo(2, "¿Cuál es 50% de 80?", new[] { "20", "30", "40" }, 2, "La mitad de ochenta es cuarenta."),
                new PreguntaNexo(3, "¿Cuál fracción es equivalente a 4/6?", new[] { "2/3", "3/4", "1/6" }, 0, "Dividir entre dos produce 2/3.")
            }),
            new NivelPreguntas(3, new[]
            {
                new PreguntaNexo(1, "Si una camisa de $200 tiene 10% de descuento, ¿cuánto pagas?", new[] { "$190", "$180", "$210" }, 1, "El 10% son $20: 200 − 20 = 180."),
                new PreguntaNexo(2, "Resuelve 2x=18.", new[] { "x=8", "x=10", "x=9" }, 2, "Dividir dieciocho entre dos da nueve."),
                new PreguntaNexo(3, "¿Qué representa la razón 3:2?", new[] { "Tres unidades por cada dos", "Cinco unidades iguales", "Dos por cada tres" }, 0, "La razón compara tres unidades con dos.")
            }),
            new NivelPreguntas(4, new[]
            {
                new PreguntaNexo(1, "¿Cuánto es 3/4 × 2/3?", new[] { "2/7", "1/2", "6/7" }, 1, "6/12 se simplifica a 1/2."),
                new PreguntaNexo(2, "Si y=2x+1, ¿cuánto vale y cuando x=3?", new[] { "6", "8", "7" }, 2, "2(3)+1=7."),
                new PreguntaNexo(3, "¿Cuál es la media de 4, 6 y 8?", new[] { "6", "5", "7" }, 0, "La suma es 18 y 18/3=6.")
            }),
            new NivelPreguntas(5, new[]
            {
                new PreguntaNexo(1, "Un precio de $500 sube 12%. ¿Cuál es el nuevo precio?", new[] { "$512", "$560", "$600" }, 1, "12% de 500 es 60; total 560."),
                new PreguntaNexo(2, "Resuelve 3(x−2)=12.", new[] { "x=4", "x=5", "x=6" }, 2, "x−2=4, así que x=6."),
                new PreguntaNexo(3, "Un mapa usa escala 1:100. ¿Cuánto representan 2 cm?", new[] { "200 cm", "20 cm", "100 cm" }, 0, "Dos centímetros por cien son doscientos centímetros.")
            })
        },
        ["argumentos"] = new[]
        {
            new NivelPreguntas(2, new[]
            {
                new PreguntaNexo(1, "¿Qué es una tesis en un texto argumentativo?", new[] { "Un saludo", "La postura principal", "Un dato aleatorio" }, 1, "La tesis plantea lo que se defiende."),
                new PreguntaNexo(2, "¿Qué fuente suele ser más verificable?", new[] { "Rumor anónimo", "Cadena sin autor", "Informe con autor y fecha" }, 2, "Identificar autor y fecha ayuda a comprobar."),
                new PreguntaNexo(3, "¿Cuál conector indica causa?", new[] { "Porque", "Sin embargo", "Además" }, 0, "Porque introduce una razón.")
            }),
            new NivelPreguntas(3, new[]
            {
                new PreguntaNexo(1, "¿Cuál oración expresa un hecho comprobable?", new[] { "El verano es hermoso.", "El agua hierve cerca de 100 °C al nivel del mar.", "El azul es el mejor color." }, 1, "Es una afirmación medible bajo condiciones definidas."),
                new PreguntaNexo(2, "¿Para qué sirve un contraargumento?", new[] { "Insultar", "Cambiar de tema", "Examinar una postura opuesta" }, 2, "Considerar objeciones fortalece el análisis."),
                new PreguntaNexo(3, "¿Qué es parafrasear?", new[] { "Explicar con tus palabras y citar", "Copiar sin citar", "Inventar cifras" }, 0, "Parafrasear reformula ideas y respeta autoría.")
            }),
            new NivelPreguntas(4, new[]
            {
                new PreguntaNexo(1, "¿Qué indica «sin embargo»?", new[] { "Consecuencia", "Contraste", "Enumeración" }, 1, "Este conector muestra oposición."),
                new PreguntaNexo(2, "¿Qué es evidencia pertinente?", new[] { "Cualquier opinión", "Una distracción", "Un dato relacionado con la tesis" }, 2, "La evidencia debe apoyar lo discutido."),
                new PreguntaNexo(3, "¿Cuál conclusión es sólida?", new[] { "Retoma postura y razones", "Introduce datos falsos", "Evita el tema" }, 0, "Una conclusión integra los argumentos.")
            }),
            new NivelPreguntas(5, new[]
            {
                new PreguntaNexo(1, "Una publicación dice «todos lo dicen». ¿Qué falta?", new[] { "Más signos", "Evidencia comprobable", "Un color llamativo" }, 1, "La popularidad no prueba la verdad."),
                new PreguntaNexo(2, "¿Cómo detectar sesgo en una fuente?", new[] { "Solo leer el título", "Confiar sin revisar", "Compararla con fuentes independientes" }, 2, "La comparación permite identificar omisiones."),
                new PreguntaNexo(3, "¿Qué debe incluir una cita bibliográfica básica?", new[] { "Autor, título y datos de publicación", "Solo una opinión", "Únicamente un enlace desconocido" }, 0, "Los datos permiten identificar la obra.")
            })
        },
        ["energia"] = new[]
        {
            new NivelPreguntas(2, new[]
            {
                new PreguntaNexo(1, "¿Qué sucede cuando aumenta la velocidad de un cuerpo, manteniendo su masa?", new[] { "Baja su energía cinética", "Aumenta su energía cinética", "No cambia" }, 1, "La energía cinética depende del cuadrado de la velocidad."),
                new PreguntaNexo(2, "¿Qué transformación realiza una lámpara?", new[] { "Luz a gasolina", "Sonido a masa", "Electricidad a luz y calor" }, 2, "La energía eléctrica se transforma en luz y calor."),
                new PreguntaNexo(3, "¿Cuál es una magnitud vectorial?", new[] { "Fuerza", "Temperatura", "Tiempo" }, 0, "La fuerza tiene magnitud y dirección.")
            }),
            new NivelPreguntas(3, new[]
            {
                new PreguntaNexo(1, "¿Cuál es la unidad de energía en el SI?", new[] { "Newton", "Julio", "Metro" }, 1, "La energía se mide en julios."),
                new PreguntaNexo(2, "¿Qué es una mezcla homogénea?", new[] { "Piedras y agua", "Arena y clavos", "Sal disuelta en agua" }, 2, "Tiene composición uniforme a simple vista."),
                new PreguntaNexo(3, "¿Qué indica una rapidez de 10 m/s?", new[] { "Diez metros cada segundo", "Diez segundos por metro", "Diez kilómetros por minuto" }, 0, "Expresa distancia recorrida por tiempo.")
            }),
            new NivelPreguntas(4, new[]
            {
                new PreguntaNexo(1, "Si F=ma, masa 2 kg y aceleración 3 m/s², ¿cuál es la fuerza?", new[] { "5 N", "6 N", "9 N" }, 1, "Dos por tres son seis newtons."),
                new PreguntaNexo(2, "¿Qué gas predomina en el aire?", new[] { "Oxígeno", "Dióxido de carbono", "Nitrógeno" }, 2, "El nitrógeno es el componente mayoritario."),
                new PreguntaNexo(3, "¿Qué sucede en una reacción de combustión?", new[] { "Se libera energía", "Desaparece la materia", "Se detiene la temperatura" }, 0, "La combustión libera energía química.")
            }),
            new NivelPreguntas(5, new[]
            {
                new PreguntaNexo(1, "Una máquina recibe 100 J y entrega 70 J útiles. Su eficiencia es...", new[] { "30%", "70%", "170%" }, 1, "70 dividido entre 100 equivale al 70%."),
                new PreguntaNexo(2, "¿Qué fenómeno explica la refracción?", new[] { "El eco", "La gravitación", "Cambio de dirección al pasar entre medios" }, 2, "La luz modifica su dirección al cambiar de medio."),
                new PreguntaNexo(3, "En un circuito en serie, si una lámpara abre el circuito...", new[] { "Se interrumpe la corriente en todo el circuito", "Las otras brillan más siempre", "No cambia nada" }, 0, "Un circuito en serie tiene una sola trayectoria.")
            })
        },
        ["digital"] = new[]
        {
            new NivelPreguntas(2, new[]
            {
                new PreguntaNexo(1, "¿Qué protege una verificación en dos pasos?", new[] { "La velocidad WiFi", "El acceso a la cuenta", "El color de la pantalla" }, 1, "Añade una comprobación al iniciar sesión."),
                new PreguntaNexo(2, "Una página pide tu contraseña por mensaje. ¿Qué haces?", new[] { "La envías", "La publicas", "Verificas por canal oficial" }, 2, "Podría tratarse de phishing."),
                new PreguntaNexo(3, "¿Qué significa huella digital en internet?", new[] { "Rastro de actividades en línea", "Tamaño del teclado", "Un tipo de archivo" }, 0, "Tus interacciones dejan información digital.")
            }),
            new NivelPreguntas(3, new[]
            {
                new PreguntaNexo(1, "¿Qué es phishing?", new[] { "Un tipo de antivirus", "Engaño para robar datos", "Una copia de seguridad" }, 1, "Busca obtener información mediante suplantación."),
                new PreguntaNexo(2, "¿Qué permiso revisarías en una app de linterna?", new[] { "Acceso a vibración", "Acceso a pantalla", "Acceso a contactos" }, 2, "Los contactos no son necesarios para encender una linterna."),
                new PreguntaNexo(3, "¿Qué es una fuente verificable?", new[] { "La que ofrece referencias comprobables", "La que grita más", "La que no tiene autor" }, 0, "Las referencias permiten contrastar.")
            }),
            new NivelPreguntas(4, new[]
            {
                new PreguntaNexo(1, "¿Cuál acción ayuda ante una filtración de contraseñas?", new[] { "Reutilizar claves", "Cambiar la clave y activar 2FA", "Compartirla con amigos" }, 1, "Actualizar credenciales disminuye riesgo."),
                new PreguntaNexo(2, "¿Qué protege la privacidad de un compañero?", new[] { "Compartir su foto sin permiso", "Publicar su dirección", "Solicitar consentimiento" }, 2, "El consentimiento respeta su información."),
                new PreguntaNexo(3, "¿Qué indica HTTPS?", new[] { "Conexión cifrada, no autenticidad total del contenido", "Que todo el sitio es verdadero", "Que no hay riesgos" }, 0, "HTTPS cifra el canal, pero no garantiza que el sitio sea legítimo.")
            }),
            new NivelPreguntas(5, new[]
            {
                new PreguntaNexo(1, "Si un mensaje pide actuar con urgencia y un enlace extraño, conviene...", new[] { "Hacer clic de inmediato", "Comprobar por otro canal", "Reenviarlo" }, 1, "La urgencia es una táctica común de fraude."),
                new PreguntaNexo(2, "¿Qué harías ante acoso digital?", new[] { "Responder agresivamente", "Ignorarlo sin documentar nunca", "Guardar evidencia, bloquear y buscar apoyo" }, 2, "La evidencia y el acompañamiento ayudan."),
                new PreguntaNexo(3, "¿Qué estrategia reduce sesgo en una búsqueda?", new[] { "Consultar fuentes diversas y confiables", "Leer solo el primer resultado", "Aceptar todos los titulares" }, 0, "Contrastar fuentes amplía la perspectiva.")
            })
        },
        ["funciones"] = new[]
        {
            new NivelPreguntas(2, new[]
            {
                new PreguntaNexo(1, "Si f(x)=x+7, f(−3) vale...", new[] { "10", "4", "−4" }, 1, "−3+7=4."),
                new PreguntaNexo(2, "Resuelve 5x=25.", new[] { "4", "6", "5" }, 2, "25/5=5."),
                new PreguntaNexo(3, "La recta y=−3x+2 tiene pendiente...", new[] { "−3", "2", "3" }, 0, "El coeficiente de x es −3.")
            }),
            new NivelPreguntas(3, new[]
            {
                new PreguntaNexo(1, "El vértice de y=x² está en...", new[] { "(1,0)", "(0,0)", "(0,1)" }, 1, "La parábola básica tiene vértice en el origen."),
                new PreguntaNexo(2, "Resuelve x²−9=0.", new[] { "Solo 3", "Solo −3", "3 y −3" }, 2, "Ambos números tienen cuadrado nueve."),
                new PreguntaNexo(3, "¿Cuál es el dominio de f(x)=1/x?", new[] { "Todos los reales excepto 0", "Solo positivos", "Todos los reales" }, 0, "Dividir entre cero no está definido.")
            }),
            new NivelPreguntas(4, new[]
            {
                new PreguntaNexo(1, "La derivada de f(x)=x² es...", new[] { "x", "2x", "x³" }, 1, "Aplicar regla de potencia da 2x."),
                new PreguntaNexo(2, "Si f(x)=2x y g(x)=x+1, (f∘g)(3) es...", new[] { "6", "7", "8" }, 2, "g(3)=4; f(4)=8."),
                new PreguntaNexo(3, "¿Qué describe la pendiente de una gráfica posición-tiempo?", new[] { "Velocidad", "Masa", "Temperatura" }, 0, "La pendiente representa variación de posición respecto al tiempo.")
            }),
            new NivelPreguntas(5, new[]
            {
                new PreguntaNexo(1, "La derivada de 3x³ es...", new[] { "3x²", "9x²", "x⁹" }, 1, "3 por 3x² = 9x²."),
                new PreguntaNexo(2, "Si una inversión crece 10% anual, $1000 después de un año serán...", new[] { "$1010", "$1200", "$1100" }, 2, "1000 × 1.10 = 1100."),
                new PreguntaNexo(3, "Resuelve log₁₀(1000).", new[] { "3", "10", "100" }, 0, "Diez elevado a tres es mil.")
            })
        },
        ["critica"] = new[]
        {
            new NivelPreguntas(2, new[]
            {
                new PreguntaNexo(1, "¿Qué es una premisa?", new[] { "Una emoción", "Una razón que apoya una conclusión", "Un tipo de letra" }, 1, "Las premisas sostienen el argumento."),
                new PreguntaNexo(2, "¿Qué es un ejemplo de generalización apresurada?", new[] { "Revisar una muestra amplia", "Contrastar fuentes", "Juzgar a todos por un caso" }, 2, "Un caso aislado no representa al conjunto."),
                new PreguntaNexo(3, "¿Qué es evidencia empírica?", new[] { "Información obtenida por observación o medición", "Un rumor", "Una preferencia" }, 0, "La evidencia empírica proviene de datos observables.")
            }),
            new NivelPreguntas(3, new[]
            {
                new PreguntaNexo(1, "En un debate, ¿qué es refutar?", new[] { "Cambiar de tema", "Responder con razones a un argumento", "Gritar más fuerte" }, 1, "Refutar implica examinar y cuestionar con razones."),
                new PreguntaNexo(2, "¿Qué debe hacerse con un conflicto de intereses?", new[] { "Ocultarlo", "Ignorarlo", "Declararlo" }, 2, "Declararlo permite evaluar posibles sesgos."),
                new PreguntaNexo(3, "¿Cuál es una inferencia válida?", new[] { "Conclusión apoyada en premisas", "Una suposición sin sustento", "Un insulto" }, 0, "La validez depende de la relación lógica.")
            }),
            new NivelPreguntas(4, new[]
            {
                new PreguntaNexo(1, "¿Qué puede indicar correlación sin causalidad?", new[] { "Que una variable causa necesariamente otra", "Que dos variables varían juntas sin demostrar causa", "Que no existen datos" }, 1, "Correlación no prueba una relación causal."),
                new PreguntaNexo(2, "¿Para qué sirve el contexto de una cita?", new[] { "Para ocultar palabras", "Para decorar", "Para evitar interpretaciones engañosas" }, 2, "El contexto permite entender el sentido original."),
                new PreguntaNexo(3, "¿Qué caracteriza una conclusión matizada?", new[] { "Reconoce límites e incertidumbre", "Afirma certeza total sin pruebas", "Descalifica fuentes" }, 0, "Reconocer límites mejora la precisión.")
            }),
            new NivelPreguntas(5, new[]
            {
                new PreguntaNexo(1, "Una gráfica omite el eje vertical. ¿Qué debes revisar?", new[] { "Su color", "La escala y los datos originales", "El tipo de letra" }, 1, "Las escalas truncadas pueden distorsionar."),
                new PreguntaNexo(2, "¿Cuál muestra es menos sesgada?", new[] { "Solo amistades", "Solo participantes voluntarios de un grupo", "Muestra representativa del universo estudiado" }, 2, "La representatividad ayuda a generalizar."),
                new PreguntaNexo(3, "Al comparar dos explicaciones, conviene...", new[] { "Evaluar evidencias y supuestos", "Elegir la más popular", "Ignorar resultados contrarios" }, 0, "Contrastar pruebas y supuestos mejora las conclusiones.")
            })
        },
        ["cientifico"] = new[]
        {
            new NivelPreguntas(2, new[]
            {
                new PreguntaNexo(1, "¿Qué es una variable dependiente?", new[] { "La que se modifica voluntariamente", "La que se mide como resultado", "La que nunca cambia" }, 1, "Es la respuesta que observamos."),
                new PreguntaNexo(2, "¿Qué significa replicar un experimento?", new[] { "Repetir sin medir", "Cambiar datos", "Repetir el procedimiento para comprobar resultados" }, 2, "La replicación pone a prueba la consistencia."),
                new PreguntaNexo(3, "¿Qué instrumento mide volumen de líquido?", new[] { "Probeta", "Regla", "Balanza" }, 0, "Una probeta graduada mide volumen.")
            }),
            new NivelPreguntas(3, new[]
            {
                new PreguntaNexo(1, "¿Qué diferencia precisión y exactitud?", new[] { "Son idénticas", "Precisión es consistencia, exactitud cercanía al valor real", "Precisión es rapidez" }, 1, "Una mide repetibilidad; otra, cercanía al valor verdadero."),
                new PreguntaNexo(2, "¿Qué significa pH menor que 7 en agua a 25 °C?", new[] { "Neutro", "Básico", "Ácido" }, 2, "Un pH inferior a siete indica acidez en esa referencia."),
                new PreguntaNexo(3, "¿Qué conserva una reacción química en un sistema cerrado?", new[] { "La masa total", "El color", "La temperatura siempre" }, 0, "La masa se conserva en un sistema cerrado.")
            }),
            new NivelPreguntas(4, new[]
            {
                new PreguntaNexo(1, "¿Qué representa la desviación estándar?", new[] { "El promedio", "La dispersión de los datos", "El número de variables" }, 1, "Mide cuánto se dispersan los valores."),
                new PreguntaNexo(2, "¿Qué es un modelo científico?", new[] { "Una opinión personal", "Un resultado infalible", "Representación para explicar o predecir" }, 2, "Los modelos simplifican sistemas para comprenderlos."),
                new PreguntaNexo(3, "¿Cuál es el objetivo de un ensayo doble ciego?", new[] { "Reducir sesgos de expectativas", "Eliminar controles", "Cambiar resultados" }, 0, "Ocultar asignación a participantes y evaluadores reduce sesgos.")
            }),
            new NivelPreguntas(5, new[]
            {
                new PreguntaNexo(1, "¿Qué indica un intervalo de confianza?", new[] { "Que el resultado es siempre exacto", "Un rango estimado según un procedimiento estadístico", "Que no existe incertidumbre" }, 1, "Expresa incertidumbre de la estimación."),
                new PreguntaNexo(2, "Si duplicas el tamaño de muestra, ¿qué suele ocurrir con el error estándar?", new[] { "Se duplica", "Permanece igual siempre", "Disminuye" }, 2, "Con otras condiciones iguales, suele disminuir."),
                new PreguntaNexo(3, "¿Qué fortalece una conclusión científica?", new[] { "Resultados reproducibles y límites claros", "Seleccionar solo datos favorables", "Evitar revisión" }, 0, "Reproducibilidad y transparencia sustentan conclusiones.")
            })
        },
        ["finanzas"] = new[]
        {
            new NivelPreguntas(2, new[]
            {
                new PreguntaNexo(1, "Si ganas $3000 y gastas $2400, ¿cuánto ahorras?", new[] { "$400", "$600", "$800" }, 1, "3000 − 2400 = 600."),
                new PreguntaNexo(2, "¿Qué es un fondo de emergencia?", new[] { "Dinero para compras impulsivas", "Una deuda", "Ahorro para imprevistos" }, 2, "Ayuda a cubrir gastos inesperados."),
                new PreguntaNexo(3, "¿Qué es una necesidad financiera?", new[] { "Alimento básico", "Un artículo de lujo opcional", "Un videojuego" }, 0, "La alimentación es una necesidad esencial.")
            }),
            new NivelPreguntas(3, new[]
            {
                new PreguntaNexo(1, "Si ahorras $100 semanales por 8 semanas, juntas...", new[] { "$600", "$800", "$1000" }, 1, "Ocho veces cien son ochocientos."),
                new PreguntaNexo(2, "¿Qué indica una tasa de interés anual?", new[] { "El número de pagos", "El saldo actual", "Porcentaje por año" }, 2, "Describe una tasa referida al año."),
                new PreguntaNexo(3, "¿Cuál es un gasto variable?", new[] { "Comidas fuera de casa", "Renta fija", "Mensualidad fija" }, 0, "Puede cambiar según decisiones y consumo.")
            }),
            new NivelPreguntas(4, new[]
            {
                new PreguntaNexo(1, "Una deuda de $1000 tiene 10% de interés simple anual. Al año debes...", new[] { "$1010", "$1100", "$1200" }, 1, "El 10% de mil es cien."),
                new PreguntaNexo(2, "¿Qué significa diversificar?", new[] { "Poner todo en un activo", "No considerar riesgos", "Distribuir entre distintas inversiones" }, 2, "Distribuir puede reducir riesgos concentrados."),
                new PreguntaNexo(3, "¿Cuál decisión ayuda a comparar créditos?", new[] { "Revisar tasa, comisiones y costo total", "Elegir solo por publicidad", "Ignorar el plazo" }, 0, "El costo total depende de varios elementos.")
            }),
            new NivelPreguntas(5, new[]
            {
                new PreguntaNexo(1, "Si $2000 crecen 5% compuesto durante dos años, tendrás...", new[] { "$2100", "$2205", "$2400" }, 1, "2000 × 1.05² = 2205."),
                new PreguntaNexo(2, "¿Qué es la inflación?", new[] { "Un descuento temporal", "Una ganancia garantizada", "Aumento general sostenido de precios" }, 2, "Reduce el poder adquisitivo cuando los ingresos no acompañan."),
                new PreguntaNexo(3, "¿Qué debe incluir un plan financiero realista?", new[] { "Metas, plazos, ingresos y riesgos", "Solo deseos", "Únicamente gastos pasados" }, 0, "Un plan integra recursos, objetivos y posibles riesgos.")
            })
        }
    };

    public static IReadOnlyList<PreguntaNexo> Obtener(LeccionNexo modulo, int nivel)
    {
        if(nivel == 1) return modulo.Preguntas;
        if(nivel is < 2 or > 5) throw new ArgumentOutOfRangeException(nameof(nivel));
        return Adicionales[modulo.Clave][nivel - 2].Preguntas;
    }
    public static int TotalNiveles => 5;
}
