using Biblioteca.Data;
using Biblioteca.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Biblioteca.Controllers
{
    [Authorize(Roles = RolesApp.Administrador)]
    public class RolesController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;

        public RolesController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var usuarios = await _userManager.Users.OrderBy(u => u.UserName).ToListAsync();
            var lista = new List<UsuarioRolViewModel>();

            foreach (var usuario in usuarios)
            {
                var roles = await _userManager.GetRolesAsync(usuario);
                lista.Add(new UsuarioRolViewModel
                {
                    Id = usuario.Id,
                    NombreUsuario = usuario.UserName ?? string.Empty,
                    Email = usuario.Email ?? string.Empty,
                    Rol = roles.FirstOrDefault() ?? "Sin rol"
                });
            }

            ViewBag.Roles = RolesApp.Todos;
            return View(lista);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Asignar(string id, string rol)
        {
            if (!RolesApp.Todos.Contains(rol))
            {
                TempData["Error"] = "El rol seleccionado no es válido.";
                return RedirectToAction(nameof(Index));
            }

            var usuario = await _userManager.FindByIdAsync(id);
            if (usuario == null) return NotFound();

            if (usuario.Id == _userManager.GetUserId(User) && rol != RolesApp.Administrador)
            {
                TempData["Error"] = "No podés quitarte a vos misma/o el rol de Administrador.";
                return RedirectToAction(nameof(Index));
            }

            var rolesActuales = await _userManager.GetRolesAsync(usuario);
            await _userManager.RemoveFromRolesAsync(usuario, rolesActuales);
            await _userManager.AddToRoleAsync(usuario, rol);
            await _userManager.UpdateSecurityStampAsync(usuario);

            TempData["Mensaje"] = $"{usuario.UserName} ahora tiene el rol {rol}. El cambio se aplica cuando vuelva a iniciar sesión.";
            return RedirectToAction(nameof(Index));
        }
    }
}
