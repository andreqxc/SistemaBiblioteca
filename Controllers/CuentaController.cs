using Biblioteca.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Biblioteca.Controllers
{
    public class CuentaController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        public CuentaController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (_signInManager.IsSignedIn(User)) return RedirectToAction("Index", "Home");

            return View(new LoginViewModel
            {
                ReturnUrl = returnUrl,
                UsuarioOCorreo = TempData["UsuarioRegistrado"] as string ?? string.Empty
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var entrada = model.UsuarioOCorreo.Trim();
            var usuario = entrada.Contains('@')
                ? await _userManager.FindByEmailAsync(entrada)
                : await _userManager.FindByNameAsync(entrada);

            if (usuario != null)
            {
                var resultado = await _signInManager.PasswordSignInAsync(usuario, model.Password, model.Recordarme, lockoutOnFailure: false);

                if (resultado.Succeeded)
                {
                    TempData["MensajeCuenta"] = $"¡Bienvenida/o, {usuario.UserName}!";

                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                        return LocalRedirect(model.ReturnUrl);

                    return RedirectToAction("Index", "Home");
                }
            }

            ModelState.AddModelError(string.Empty, "Usuario, correo o contraseña incorrectos. Verificá tus datos e intentá de nuevo.");
            return View(model);
        }

        [HttpGet]
        public IActionResult Registro()
        {
            if (_signInManager.IsSignedIn(User)) return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registro(RegistroViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var usuario = new IdentityUser
            {
                UserName = model.NombreUsuario.Trim(),
                Email = model.Email.Trim()
            };

            var resultado = await _userManager.CreateAsync(usuario, model.Password);

            if (resultado.Succeeded)
            {
                TempData["UsuarioRegistrado"] = usuario.UserName;
                TempData["RegistroExitoso"] = "Tu cuenta se creó correctamente. Ya podés iniciar sesión.";
                return RedirectToAction(nameof(Login));
            }

            foreach (var error in resultado.Errors)
                ModelState.AddModelError(string.Empty, error.Description);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            TempData["MensajeCuenta"] = "Cerraste sesión correctamente.";
            return RedirectToAction("Index", "Home");
        }
    }
}
