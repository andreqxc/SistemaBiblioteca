using Biblioteca.Models;
using Microsoft.AspNetCore.Mvc;

namespace Biblioteca.Controllers
{
    public class AutoresController : Controller
    {
        public IActionResult Index()
        {
            var autores = new List<Autor>
            {
                new Autor { Id = 1, Nombre = "Gabriel", Apellido = "García Márquez", Nacionalidad = "Colombiana", FechaNacimiento = new DateTime(1927, 3, 6), Activo = true },
                new Autor { Id = 2, Nombre = "Isabel", Apellido = "Allende", Nacionalidad = "Chilena", FechaNacimiento = new DateTime(1942, 8, 2), Activo = true },
                new Autor { Id = 3, Nombre = "Mario", Apellido = "Vargas Llosa", Nacionalidad = "Peruana", FechaNacimiento = new DateTime(1936, 3, 28), Activo = false },
                new Autor { Id = 4, Nombre = "Jorge Luis", Apellido = "Borges", Nacionalidad = "Argentina", FechaNacimiento = new DateTime(1899, 8, 24), Activo = false },
                new Autor { Id = 5, Nombre = "Claudia", Apellido = "Lars", Nacionalidad = "Salvadoreña", FechaNacimiento = new DateTime(1899, 12, 20), Activo = true }
            };

            return View(autores);
        }
    }
}
