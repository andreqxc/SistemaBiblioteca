using Biblioteca.Data;
using Biblioteca.Models;
using Microsoft.AspNetCore.Mvc;

namespace Biblioteca.Controllers
{
    public class AutoresController : Controller
    {
        private const string Archivo = "autores.json";

        private static List<Autor> _autores = JsonStore.Cargar(Archivo, new List<Autor>
        {
            new Autor { Id = 1, Nombre = "Gabriel", Apellido = "García Márquez", Nacionalidad = "Colombiana", FechaNacimiento = new DateTime(1927, 3, 6), Activo = true },
            new Autor { Id = 2, Nombre = "Isabel", Apellido = "Allende", Nacionalidad = "Chilena", FechaNacimiento = new DateTime(1942, 8, 2), Activo = true },
            new Autor { Id = 3, Nombre = "Mario", Apellido = "Vargas Llosa", Nacionalidad = "Peruana", FechaNacimiento = new DateTime(1936, 3, 28), Activo = false },
            new Autor { Id = 4, Nombre = "Jorge Luis", Apellido = "Borges", Nacionalidad = "Argentina", FechaNacimiento = new DateTime(1899, 8, 24), Activo = false },
            new Autor { Id = 5, Nombre = "Claudia", Apellido = "Lars", Nacionalidad = "Salvadoreña", FechaNacimiento = new DateTime(1899, 12, 20), Activo = true }
        });

        public static List<Autor> Autores => _autores;

        public IActionResult Index()
        {
            return View(_autores);
        }

        public IActionResult Edit(int id)
        {
            var autor = _autores.FirstOrDefault(a => a.Id == id);
            if (autor == null) return NotFound();
            return View(autor);
        }

        [HttpPost]
        public IActionResult Edit(int id, Autor model)
        {
            var autor = _autores.FirstOrDefault(a => a.Id == id);
            if (autor == null) return NotFound();

            autor.Nombre = model.Nombre;
            autor.Apellido = model.Apellido;
            autor.Nacionalidad = model.Nacionalidad;
            autor.FechaNacimiento = model.FechaNacimiento;
            autor.Activo = model.Activo;

            JsonStore.Guardar(Archivo, _autores);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var autor = _autores.FirstOrDefault(a => a.Id == id);
            if (autor != null) _autores.Remove(autor);

            JsonStore.Guardar(Archivo, _autores);
            return RedirectToAction("Index");
        }
    }
}
