using Biblioteca.Data;
using Biblioteca.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace Biblioteca.ViewComponents
{
    public class AccesosRapidosViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var accesos = new List<AccesoRapido>
            {
                new("Inicio", "Home", "Index", "🏠"),
                new("Categorías", "Categorias", "Index", "🏷️"),
                new("Autores", "Autores", "Index", "✍️")
            };

            if (User.Identity?.IsAuthenticated == true)
            {
                accesos.Insert(1, new AccesoRapido("Libros", "Libros", "Index", "📚"));
            }

            if (User.IsInRole(RolesApp.Admin))
            {
                accesos.Add(new AccesoRapido("Roles", "Roles", "Index", "🛡️"));
            }

            return View(accesos);
        }
    }
}
