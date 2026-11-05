using NEXO_EDU.Models;

namespace NEXO_EDU.Services;

// Banco curricular de demostración: preguntas revisables, sin contenido generado
// al vuelo ni respuestas correctas enviadas a las vistas.
public sealed record PreguntaNexo(int Numero, string Enunciado, string[] Opciones, int Correcta, string Explicacion);
public sealed record LeccionNexo(string Nivel, string Clave, string Materia, string Titulo, string Descripcion, string Icono, IReadOnlyList<PreguntaNexo> Preguntas);
public sealed record RespuestaNexo(int Numero, string Enunciado, string Correcta, string Explicacion, bool Acertada);
public sealed record RevisionLeccionNexo(LeccionNexo Leccion, int Aciertos, IReadOnlyList<RespuestaNexo> Respuestas, int Experiencia)
{
    public int Total => Leccion.Preguntas.Count;
    public int Porcentaje => Total == 0 ? 0 : (int)Math.Round(Aciertos * 100.0 / Total);
    public string Recomendacion => Porcentaje < 50 ? "Revisa las explicaciones y vuelve a intentarlo."
        : Porcentaje < 80 ? "Vas bien: practica otra vez para fortalecer tus ideas."
        : "¡Muy buen trabajo! Explora otra materia para continuar.";
}
public static class CatalogoLecciones
{
    public static readonly IReadOnlyList<LeccionNexo> Todas = new[]
    {
        new LeccionNexo("primaria", "numeros", "Matemáticas", "El reino de los números", "Sumas, restas y patrones para explorar.", "🔢", new []
        {
            new PreguntaNexo(1, "Si tienes 6 manzanas y regalas 2, ¿cuántas quedan?", new [] { "4", "3", "8" }, 0, "Seis menos dos son cuatro."),
            new PreguntaNexo(2, "¿Cuál número sigue? 5, 10, 15, ...", new [] { "16", "20", "25" }, 1, "La secuencia aumenta de cinco en cinco."),
            new PreguntaNexo(3, "¿Cuánto es 4 + 7?", new [] { "10", "12", "11" }, 2, "Cuatro más siete es once."),
            new PreguntaNexo(4, "¿Qué número es mayor?", new [] { "19", "12", "9" }, 0, "Diecinueve es el más grande.")
        }),
        new LeccionNexo("primaria", "lectura", "Español", "Detectives de lectura", "Comprende historias, palabras y personajes.", "📖", new []
        {
            new PreguntaNexo(1, "En «Ana cuida una planta», ¿quién cuida la planta?", new [] { "Una planta", "Ana", "Su maestro" }, 1, "Ana realiza la acción de cuidar."),
            new PreguntaNexo(2, "¿Qué palabra es un sustantivo?", new [] { "Correr", "Azul", "Casa" }, 2, "Casa nombra un lugar u objeto."),
            new PreguntaNexo(3, "¿Cuál oración expresa una pregunta?", new [] { "¿Dónde está mi libro?", "Mi libro es rojo.", "Lee el libro." }, 0, "Los signos de interrogación señalan una pregunta."),
            new PreguntaNexo(4, "¿Cuál es el antónimo de «grande»?", new [] { "Enorme", "Pequeño", "Gigante" }, 1, "Pequeño indica tamaño opuesto.")
        }),
        new LeccionNexo("primaria", "naturaleza", "Ciencias", "Exploradores de la naturaleza", "Descubre plantas, agua y seres vivos.", "🌱", new []
        {
            new PreguntaNexo(1, "¿Qué necesitan las plantas para crecer?", new [] { "Solo piedras", "Agua y luz", "Solo juguetes" }, 1, "El agua y la luz favorecen el crecimiento."),
            new PreguntaNexo(2, "¿En qué estado está el hielo?", new [] { "Sólido", "Gaseoso", "Líquido" }, 0, "El hielo es agua en estado sólido."),
            new PreguntaNexo(3, "¿Qué sentido usamos para escuchar?", new [] { "Vista", "Gusto", "Oído" }, 2, "Con el oído percibimos sonidos."),
            new PreguntaNexo(4, "¿Cuál animal nace de un huevo?", new [] { "Gato", "Gallina", "Perro" }, 1, "Las gallinas ponen huevos.")
        }),
        new LeccionNexo("primaria", "convivencia", "Convivencia", "Superpoderes para convivir", "Practica respeto, empatía y cooperación.", "🤝", new []
        {
            new PreguntaNexo(1, "Si una compañera está triste, ¿qué puedes hacer?", new [] { "Escucharla", "Burlarte", "Ignorarla" }, 0, "Escuchar y acompañar muestra empatía."),
            new PreguntaNexo(2, "Si dos personas quieren hablar, lo mejor es...", new [] { "Gritar", "Tomar turnos", "Empujar" }, 1, "Tomar turnos permite escuchar."),
            new PreguntaNexo(3, "¿Cómo cuidamos un espacio compartido?", new [] { "Dejando basura", "Rompiendo objetos", "Recogiendo materiales" }, 2, "Cuidar los espacios beneficia a todos."),
            new PreguntaNexo(4, "Si cometes un error, lo adecuado es...", new [] { "Reconocerlo y aprender", "Culpar a otra persona", "Ocultarlo siempre" }, 0, "Reconocer errores ayuda a mejorar.")
        }),
        new LeccionNexo("secundaria", "fracciones", "Matemáticas", "Misión fracciones y porcentajes", "Resuelve operaciones en situaciones cotidianas.", "📐", new []
        {
            new PreguntaNexo(1, "¿Cuánto es 3/4 + 1/8?", new [] { "4/12", "7/8", "1/2" }, 1, "3/4 son 6/8; más 1/8 suman 7/8."),
            new PreguntaNexo(2, "¿Cuál es el 20% de 150?", new [] { "30", "20", "45" }, 0, "Multiplica 150 por 0.20: da 30."),
            new PreguntaNexo(3, "Si 4 cuadernos cuestan $80, ¿cuánto cuestan 6?", new [] { "$100", "$140", "$120" }, 2, "Cada cuaderno cuesta $20; seis cuestan $120."),
            new PreguntaNexo(4, "¿Cuánto es 0.6 expresado como fracción simplificada?", new [] { "6/100", "3/5", "1/6" }, 1, "6/10 se simplifica dividiendo entre 2: 3/5.")
        }),
        new LeccionNexo("secundaria", "argumentos", "Español", "Laboratorio de argumentos", "Distingue ideas, evidencia y opiniones.", "📝", new []
        {
            new PreguntaNexo(1, "¿Cuál enunciado es una opinión?", new [] { "La Tierra orbita el Sol.", "La novela es fascinante.", "Un triángulo tiene tres lados." }, 1, "«Fascinante» expresa una valoración personal."),
            new PreguntaNexo(2, "¿Qué fortalece un argumento?", new [] { "Un dato verificable", "Un rumor", "Una repetición" }, 0, "La evidencia comprobable apoya una postura."),
            new PreguntaNexo(3, "¿Qué resume mejor un texto?", new [] { "Copiar cada oración", "Agregar un tema diferente", "Recuperar las ideas principales" }, 2, "Un resumen conserva las ideas esenciales."),
            new PreguntaNexo(4, "¿Cuál conector indica contraste?", new [] { "Además", "Sin embargo", "Por ejemplo" }, 1, "«Sin embargo» introduce una idea opuesta.")
        }),
        new LeccionNexo("secundaria", "energia", "Ciencias", "Energía en movimiento", "Relaciona fuerza, energía y cambios.", "⚡", new []
        {
            new PreguntaNexo(1, "La energía de un objeto en movimiento es...", new [] { "Cinética", "Química", "Nuclear" }, 0, "La energía cinética depende del movimiento."),
            new PreguntaNexo(2, "¿Qué unidad se usa para medir la fuerza?", new [] { "Litro", "Newton", "Segundo" }, 1, "La fuerza se expresa en newtons."),
            new PreguntaNexo(3, "¿Cuál es un cambio químico?", new [] { "Romper papel", "Derretir hielo", "Oxidar hierro" }, 2, "La oxidación forma nuevas sustancias."),
            new PreguntaNexo(4, "¿Qué fuente de energía es renovable?", new [] { "Solar", "Carbón", "Petróleo" }, 0, "La energía solar se renueva naturalmente.")
        }),
        new LeccionNexo("secundaria", "digital", "Ciudadanía digital", "Navega con criterio", "Aprende a proteger datos y evaluar fuentes.", "🔐", new []
        {
            new PreguntaNexo(1, "¿Qué contraseña es más segura?", new [] { "123456", "Una frase larga y única", "Tu nombre" }, 1, "Una contraseña larga, única y difícil de adivinar protege mejor."),
            new PreguntaNexo(2, "Antes de compartir una noticia conviene...", new [] { "Verificar la fuente", "Reenviarla sin leer", "Cambiar el título" }, 0, "Verificar la fuente reduce la desinformación."),
            new PreguntaNexo(3, "Si recibes acoso en línea, lo adecuado es...", new [] { "Responder con amenazas", "Compartir datos privados", "Guardar evidencia y pedir ayuda" }, 2, "Documentar y buscar apoyo de un adulto de confianza es más seguro."),
            new PreguntaNexo(4, "¿Qué dato personal no debes publicar?", new [] { "Una receta", "Tu domicilio", "Una opinión de libro" }, 1, "Tu domicilio puede comprometer la privacidad.")
        }),
        new LeccionNexo("preparatoria", "funciones", "Matemáticas", "Funciones para decidir", "Modela relaciones, pendientes y ecuaciones.", "ƒ(x)", new []
        {
            new PreguntaNexo(1, "Si f(x)=3x−2, ¿cuánto vale f(5)?", new [] { "13", "15", "11" }, 0, "3 por 5 menos 2 es 13."),
            new PreguntaNexo(2, "¿Cuál es la pendiente de y=4x+7?", new [] { "7", "−4", "4" }, 2, "En y=mx+b, m es la pendiente."),
            new PreguntaNexo(3, "Resuelve 2x+5=17.", new [] { "5", "6", "7" }, 1, "Resta cinco y divide entre dos: x=6."),
            new PreguntaNexo(4, "Si f(x)=x²−1, ¿cuánto vale f(−2)?", new [] { "3", "−5", "5" }, 0, "(−2)²−1=4−1=3.")
        }),
        new LeccionNexo("preparatoria", "critica", "Comunicación", "Pensamiento crítico", "Evalúa argumentos, sesgos y evidencias.", "🧠", new []
        {
            new PreguntaNexo(1, "¿Qué caracteriza a una fuente primaria?", new [] { "Analiza textos ajenos", "Aporta evidencia original", "Resume rumores" }, 1, "Una fuente primaria presenta información original."),
            new PreguntaNexo(2, "¿Qué error contiene «todos lo creen, así que es verdad»?", new [] { "Apelación a la popularidad", "Evidencia estadística", "Cita directa" }, 0, "La popularidad no demuestra veracidad."),
            new PreguntaNexo(3, "¿Qué mejora una hipótesis?", new [] { "Que no pueda comprobarse", "Que dependa de gustos", "Que sea contrastable" }, 2, "Una hipótesis útil puede ponerse a prueba."),
            new PreguntaNexo(4, "¿Qué es citar una fuente?", new [] { "Copiar sin atribuir", "Reconocer el origen de la información", "Ocultar la autoría" }, 1, "Citar permite rastrear y reconocer las ideas.")
        }),
        new LeccionNexo("preparatoria", "cientifico", "Ciencias", "Método científico aplicado", "Diseña experimentos y analiza variables.", "🔬", new []
        {
            new PreguntaNexo(1, "¿Qué variable cambia deliberadamente el investigador?", new [] { "Independiente", "Dependiente", "Constante" }, 0, "La independiente es la variable manipulada."),
            new PreguntaNexo(2, "¿Para qué sirve un grupo de control?", new [] { "Para evitar mediciones", "Para comparar resultados", "Para aumentar el sesgo" }, 1, "Ofrece un punto de comparación."),
            new PreguntaNexo(3, "Si los datos contradicen una hipótesis, se debe...", new [] { "Ocultarlos", "Inventar otros", "Revisar la hipótesis" }, 2, "Los datos permiten corregir explicaciones."),
            new PreguntaNexo(4, "¿Cuál medida reduce sesgos experimentales?", new [] { "Repetir pruebas", "Cambiar respuestas", "Excluir resultados incómodos" }, 0, "Repetir permite comprobar consistencia.")
        }),
        new LeccionNexo("preparatoria", "finanzas", "Proyecto de vida", "Decisiones financieras", "Construye un presupuesto y evalúa alternativas.", "💼", new []
        {
            new PreguntaNexo(1, "Si ingresas $2500 y gastas $2100, ¿cuánto ahorras?", new [] { "$300", "$400", "$500" }, 1, "2500 menos 2100 son 400."),
            new PreguntaNexo(2, "¿Qué es un gasto fijo?", new [] { "Pago mensual de renta", "Compra ocasional de regalo", "Un descuento inesperado" }, 0, "La renta suele ser recurrente y previsible."),
            new PreguntaNexo(3, "¿Qué ayuda a prevenir deudas excesivas?", new [] { "Gastar antes de calcular", "Ignorar comisiones", "Planear un presupuesto" }, 2, "Presupuestar facilita decidir con información."),
            new PreguntaNexo(4, "¿Qué describe mejor el interés compuesto?", new [] { "Nunca aumenta", "Genera interés también sobre intereses previos", "Solo aplica a efectivo" }, 1, "El capital acumulado puede producir intereses adicionales.")
        })
    };

    public static IReadOnlyList<LeccionNexo> PorNivel(string? nivel)
        => Todas.Where(x => x.Nivel == NivelDemo.Normalizar(nivel)).ToArray();

    public static LeccionNexo? Buscar(string? nivel, string? clave)
        => PorNivel(nivel).FirstOrDefault(x => string.Equals(x.Clave, clave, StringComparison.OrdinalIgnoreCase));

    public static RevisionLeccionNexo Revisar(LeccionNexo leccion, IReadOnlyDictionary<int,int> respuestas)
    {
        var detalle = leccion.Preguntas.Select(p => new RespuestaNexo(p.Numero, p.Enunciado,
            p.Opciones[p.Correcta], p.Explicacion,
            respuestas.TryGetValue(p.Numero, out var seleccion) && seleccion == p.Correcta)).ToArray();
        var aciertos = detalle.Count(x => x.Acertada);
        return new(leccion, aciertos, detalle, 10 + aciertos * 10);
    }
}
