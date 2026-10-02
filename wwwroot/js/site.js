// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

document.querySelectorAll("[data-toggle-password]").forEach(function (boton) {
    boton.addEventListener("click", function () {
        var input = boton.parentElement.querySelector("input");
        var oculto = input.type === "password";
        input.type = oculto ? "text" : "password";
        boton.textContent = oculto ? "Ocultar" : "Mostrar";
        boton.setAttribute("aria-label", oculto ? "Ocultar contraseña" : "Mostrar contraseña");
    });
});
