// Gestos locales de NEXO PET. Sin audio, red, permisos ni telemetría.
(() => {
  'use strict';
  const frases = {
    aguila: { wave: '¡Hola, explorador!', jump: '¡Volemos hacia nuevos retos!', celebrate: '¡Tus alas crecen contigo!' },
    coraje: { wave: '¡Hola! Vamos juntos.', jump: '¡Aunque dé nervios, lo intento!', celebrate: '¡Lo logramos con valentía!' },
    robot: { wave: '¡Saludos, estudiante!', jump: '¡Modo energía activado!', celebrate: '¡Objetivo alcanzado!' }
  };
  document.querySelectorAll('[data-pet-gesture]').forEach(btn => {
    btn.addEventListener('click', () => {
      const mascot = document.querySelector('.pet-studio__stage .nexo-mascot');
      if (!mascot) return;
      const gesture = btn.dataset.petGesture;
      if (!['wave','jump','celebrate'].includes(gesture)) return;
      const status = mascot.querySelector('.nexo-mascot__speech');
      mascot.classList.remove('is-wave', 'is-jump', 'is-celebrate', 'is-talking');
      // Reproducir nuevamente la animación si se pulsa dos veces el mismo gesto.
      void mascot.offsetWidth;
      mascot.classList.add('is-' + gesture, 'is-talking');
      if (status) status.textContent = frases[mascot.dataset.mascot]?.[gesture] || '¡Sigamos aprendiendo!';
      clearTimeout(mascot._petTimer);
      mascot._petTimer = setTimeout(() => {
        mascot.classList.remove('is-wave', 'is-jump', 'is-celebrate', 'is-talking');
        if (status) status.textContent = '';
      }, 2200);
    });
  });
})();
