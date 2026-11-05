// Acompañante NEXO PET: mensajes locales por contexto, sin red ni escucha de datos.
// Aparece 10 s, descansa 35 s y vuelve a aparecer mientras está abierta la página.
(() => {
  'use strict';
  const host = document.querySelector('[data-pet-companion]');
  if (!host) return;
  const text = host.querySelector('[data-pet-text]');
  const name = host.dataset.petName || 'NEXO PET';
  const student = host.dataset.studentName || 'estudiante';
  const type = host.dataset.petType;
  const context = host.dataset.petContext;
  const close = host.querySelector('[data-pet-dismiss]');
  const prefersLessMotion = window.matchMedia?.('(prefers-reduced-motion: reduce)').matches;
  let dismissed = false, visible = false, displayTimer, cycleTimer, index = 0;

  const motivational = {
    aguila: ['¡Cada intento te ayuda a volar más alto!', 'Aprender es una aventura. ¡Vamos juntos!', 'Confía en tus alas y prueba otra vez.'],
    coraje: ['Ser valiente también es intentar de nuevo.', '¡Lo estás haciendo muy bien, paso a paso!', 'Si algo cuesta, podemos practicarlo juntos.'],
    robot: ['¡Objetivo activado: seguir aprendiendo!', 'Un desafío a la vez. Vas construyendo habilidades.', 'Los errores también aportan información útil.']
  };
  const base = motivational[type] || motivational.aguila;
  const messages = context === 'retry'
    ? ['¡No te rindas! Equivocarte también es aprender.', 'Respira, revisa la explicación y vuelve a intentarlo.', base[2]]
    : context === 'success'
      ? ['¡Bien hecho! Tu esfuerzo está dando resultados.', '¡Celebramos cada pequeño avance!', base[0]]
      : context === 'activity'
        ? ['Lee con calma: tienes tiempo para pensar.', 'Si una pregunta cuesta, prueba con otra estrategia.', base[0]]
        : context === 'welcome'
          ? [`¡Hola, ${student}! Soy ${name} y aprenderemos juntos.`, base[0], base[1]]
          : [base[0], base[1], base[2]];

  const hide = () => {
    visible = false;
    host.classList.remove('is-visible');
    window.clearTimeout(displayTimer);
  };
  const show = () => {
    if (dismissed || document.hidden) return;
    // No interrumpir cuando el alumno está escribiendo o respondiendo.
    const focused = document.activeElement;
    if (focused?.matches?.('input, textarea, select, [contenteditable="true"]')) return;
    text.textContent = messages[index++ % messages.length];
    host.classList.add('is-visible');
    visible = true;
    displayTimer = window.setTimeout(hide, 10000);
  };
  // Los resultados se comentan en cuanto el alumno los consulta.
  cycleTimer = window.setTimeout(show, context === 'success' || context === 'retry' ? 650 : 1100);
  const repeat = window.setInterval(() => {
    if (dismissed) return;
    if (visible) hide();
    show();
  }, 45000);
  close?.addEventListener('click', () => {
    dismissed = true;
    window.clearTimeout(cycleTimer);
    window.clearTimeout(displayTimer);
    window.clearInterval(repeat);
    hide();
  });
  document.addEventListener('visibilitychange', () => {
    if (document.hidden) hide();
  });
  // Reducir el movimiento implica desactivar transiciones, no el apoyo textual.
  if (prefersLessMotion) host.classList.add('reduce-motion');
})();
