using Biblioteca.Models;
using Biblioteca.Services;
using Microsoft.AspNetCore.Mvc;

namespace Biblioteca.Controllers
{
    public class AutoresController : Controller
    {
        private readonly IAutorService _autorService;

        public AutoresController(IAutorService autorService)
        {
            _autorService = autorService;
        }

        public IActionResult Index()
        {
            return View(_autorService.ObtenerTodos());
        }

        public IActionResult Edit(int id)
        {
            var autor = _autorService.ObtenerPorId(id);
            if (autor == null) return NotFound();
            return View(autor);
        }

        [HttpPost]
        public IActionResult Edit(int id, Autor model)
        {
            var autor = _autorService.ObtenerPorId(id);
            if (autor == null) return NotFound();

            model.Id = id;
            _autorService.Actualizar(model);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            _autorService.Eliminar(id);
            return RedirectToAction("Index");
        }
    }
}
