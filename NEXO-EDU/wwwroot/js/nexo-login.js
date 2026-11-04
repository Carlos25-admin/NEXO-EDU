document.addEventListener("DOMContentLoaded", () => {
  const buttons = document.querySelectorAll("[data-demo-email]");
  const email = document.getElementById("Email");
  buttons.forEach(button => button.addEventListener("click", () => {
    buttons.forEach(b => b.classList.remove("chosen"));
    button.classList.add("chosen");
    if (email) { email.value = button.dataset.demoEmail || ""; email.focus(); }
  }));
});
