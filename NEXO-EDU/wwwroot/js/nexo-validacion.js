// Validación progresiva: informa, NO altera silenciosamente lo escrito.
// La validación definitiva se repite en los modelos/controladores ASP.NET Core.
document.addEventListener("DOMContentLoaded", () => {
  const fields = document.querySelectorAll("[data-nexo-validate]");
  const rules = {
    nombre: {
      test: value => /^[\p{L}\p{M}]+(?:[ '\u2019-][\p{L}\p{M}]+)*$/u.test(value),
      message: "Escribe solo letras, acentos y espacios. Se permiten guiones y apóstrofes entre palabras."
    },
    institucion: {
      test: value => /^(?=.*[\p{L}\p{M}])[\p{L}\p{M}\p{N} .,'\u2019()#&/-]+$/u.test(value),
      message: "Escribe el nombre de la institución con letras, números y signos habituales."
    },
    grupo: {
      test: value => /^[\p{L}\p{N}]{1,12}$/u.test(value),
      message: "El grupo admite hasta 12 letras o números, sin espacios ni símbolos."
    },
    usuario: {
      test: value => /^[a-z][a-z0-9]{2,29}$/.test(value),
      message: "De 3 a 30 caracteres, letras minúsculas y números; empieza con una letra."
    },
    clave: {
      test: value => /^(?=.*[A-Z])(?=.*[a-z])(?=.*[0-9])[A-Za-z0-9]{12,64}$/.test(value),
      message: "De 12 a 64 caracteres con mayúsculas, minúsculas y números, sin espacios ni símbolos."
    },
    email: {
      test: value => /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(value),
      message: "Introduce un correo electrónico válido."
    }
  };
  for (const input of fields) {
    const rule = rules[input.dataset.nexoValidate];
    if (!rule) continue;
    const helper = document.createElement("small");
    helper.className = "nexo-field-feedback";
    helper.setAttribute("aria-live", "polite");
    helper.id = input.id + "-feedback";
    input.insertAdjacentElement("afterend", helper);
    input.setAttribute("aria-describedby", [
      input.getAttribute("aria-describedby"), helper.id
    ].filter(Boolean).join(" "));
    const evaluate = () => {
      // Validar únicamente los campos visibles/activos. Las contraseñas
      // generadas por el sistema no necesitan tener un valor manual.
      if (input.disabled || input.closest("[hidden]")) {
        input.setCustomValidity("");
        helper.textContent = "";
        return;
      }
      const val = input.value.trim().replace(/\s+/g, " ");
      const valid = val.length === 0 ? !input.required : rule.test(val);
      const msg = valid ? "" : (val.length === 0 ? "Este campo es obligatorio." : rule.message);
      input.setCustomValidity(msg);
      input.classList.toggle("nexo-field-invalid", !!msg);
      helper.textContent = msg;
    };
    input.addEventListener("input", evaluate);
    input.addEventListener("blur", evaluate);
    input.form?.addEventListener("submit", evaluate);
  }
});
