using Biblioteca.Data;
using Biblioteca.Models;
using Microsoft.AspNetCore.Mvc;

namespace Biblioteca.Controllers
{
    public class CategoriasController : Controller
    {
        private readonly CategoriaRepositorio _repositorio;

        public CategoriasController(CategoriaRepositorio repositorio)
        {
            _repositorio = repositorio;
        }

        public IActionResult Index()
        {
            return View(_repositorio.Listar());
        }

        public IActionResult Crear()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Crear(Categoria categoria)
        {
            if (!ModelState.IsValid) return View(categoria);

            _repositorio.Insertar(categoria);
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            var categoria = _repositorio.ObtenerPorId(id);
            if (categoria == null) return NotFound();
            return View(categoria);
        }

        [HttpPost]
        public IActionResult Edit(int id, Categoria model)
        {
            var categoria = _repositorio.ObtenerPorId(id);
            if (categoria == null) return NotFound();

            categoria.Nombre = model.Nombre;
            categoria.Descripcion = model.Descripcion;

            _repositorio.Actualizar(categoria);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            _repositorio.Eliminar(id);
            return RedirectToAction("Index");
        }
    }
}
