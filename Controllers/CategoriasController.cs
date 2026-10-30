using Biblioteca.Data;
using Biblioteca.Models;
using Microsoft.AspNetCore.Mvc;

namespace Biblioteca.Controllers
{
    public class CategoriasController : Controller
    {
        private readonly CategoriaRepositorio _repositorio;
        private readonly ILogger<CategoriasController> _logger;

        public CategoriasController(CategoriaRepositorio repositorio, ILogger<CategoriasController> logger)
        {
            _repositorio = repositorio;
            _logger = logger;
        }

        private string UsuarioActual => User.Identity?.Name ?? "anónimo";

        public IActionResult Index()
        {
            _logger.LogInformation("Inicio de consulta del listado de categorías por {Usuario}", UsuarioActual);
            var categorias = _repositorio.Listar();
            _logger.LogInformation("Listado de categorías consultado: {Cantidad} registros", categorias.Count);
            return View(categorias);
        }

        public IActionResult Crear()
        {
            _logger.LogInformation("Apertura del formulario de creación de categoría por {Usuario}", UsuarioActual);
            return View();
        }

        [HttpPost]
        public IActionResult Crear(Categoria categoria)
        {
            _logger.LogInformation("Inicio de creación de categoría por {Usuario}", UsuarioActual);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Creación de categoría rechazada por datos no válidos ({Errores} errores de validación)", ModelState.ErrorCount);
                return View(categoria);
            }

            try
            {
                _repositorio.Insertar(categoria);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear la categoría {Nombre}", categoria.Nombre);
                throw;
            }

            _logger.LogInformation("Categoría {Nombre} creada correctamente por {Usuario}", categoria.Nombre, UsuarioActual);
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            _logger.LogInformation("Apertura del formulario de edición de la categoría {CategoriaId} por {Usuario}", id, UsuarioActual);
            var categoria = _repositorio.ObtenerPorId(id);
            if (categoria == null)
            {
                _logger.LogWarning("No se encontró la categoría {CategoriaId} para editarla", id);
                return NotFound();
            }
            return View(categoria);
        }

        [HttpPost]
        public IActionResult Edit(int id, Categoria model)
        {
            _logger.LogInformation("Inicio de edición de la categoría {CategoriaId} por {Usuario}", id, UsuarioActual);
            var categoria = _repositorio.ObtenerPorId(id);
            if (categoria == null)
            {
                _logger.LogWarning("No se encontró la categoría {CategoriaId} al intentar actualizarla", id);
                return NotFound();
            }

            try
            {
                categoria.Nombre = model.Nombre;
                categoria.Descripcion = model.Descripcion;
                _repositorio.Actualizar(categoria);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar la categoría {CategoriaId}", id);
                throw;
            }

            _logger.LogInformation("Categoría {CategoriaId} actualizada correctamente por {Usuario}", id, UsuarioActual);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            _logger.LogInformation("Inicio de eliminación de la categoría {CategoriaId} por {Usuario}", id, UsuarioActual);

            if (_repositorio.ObtenerPorId(id) == null)
            {
                _logger.LogWarning("No se encontró la categoría {CategoriaId} al intentar eliminarla", id);
                return RedirectToAction("Index");
            }

            try
            {
                _repositorio.Eliminar(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar la categoría {CategoriaId}", id);
                throw;
            }

            _logger.LogInformation("Categoría {CategoriaId} eliminada correctamente por {Usuario}", id, UsuarioActual);
            return RedirectToAction("Index");
        }
    }
}
