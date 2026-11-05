document.addEventListener("DOMContentLoaded", () => {
  const buttons = [...document.querySelectorAll("[data-demo-email]")];
  const email = document.getElementById("Email");
  const password = document.getElementById("Password");
  const reveal = document.querySelector("[data-password-toggle]");
  const greeting = document.querySelector("[data-login-greeting]");
  const buddy = document.querySelector(".login-buddy");
  let greetingTimer;

  const greetings = [
    "¡Hola! Soy<br/><strong>NEXO PET</strong>",
    "¡Qué bueno<br/><strong>verte!</strong>",
    "¡Vamos a<br/><strong>aprender!</strong>"
  ];
  let greetingIndex = 0;
  const greet = () => {
    if (!greeting) return;
    greetingIndex = (greetingIndex + 1) % greetings.length;
    greeting.innerHTML = greetings[greetingIndex];
    buddy?.classList.remove("is-greeting");
    void buddy?.offsetWidth;
    buddy?.classList.add("is-greeting");
  };
  buttons.forEach(button => button.addEventListener("click", () => {
    buttons.forEach(b => {
      b.classList.remove("chosen");
      b.setAttribute("aria-pressed","false");
    });
    button.classList.add("chosen");
    button.setAttribute("aria-pressed","true");
    if (email) {
      // El perfil solo ayuda a rellenar el usuario; la contraseña siempre
      // debe ingresarse y validarse por Identity en el servidor.
      email.value = button.dataset.demoEmail || "";
      email.focus();
    }
    greet();
  }));
  buttons.forEach(button => button.setAttribute("aria-pressed",
    String(button.classList.contains("chosen"))));

  reveal?.addEventListener("click", () => {
    if (!password) return;
    const show = password.type === "password";
    password.type = show ? "text" : "password";
    reveal.setAttribute("aria-pressed", String(show));
    reveal.setAttribute("aria-label", show ? "Ocultar contraseña" : "Mostrar contraseña");
    reveal.querySelector("span").textContent = show ? "◌" : "◉";
  });

  // Animación ligera sin peticiones, audio ni lectura de contraseñas.
  const reduceMotion = window.matchMedia("(prefers-reduced-motion: reduce)");
  if (!reduceMotion.matches) {
    greetingTimer = window.setInterval(() => {
      if (!document.hidden) greet();
    }, 6500);
    document.addEventListener("visibilitychange", () => {
      if (document.hidden) buddy?.classList.remove("is-greeting");
    });
  }
});
