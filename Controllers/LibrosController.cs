using Biblioteca.Data;
using Biblioteca.Models;
using Biblioteca.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Biblioteca.Controllers
{
    [Authorize]
    public class LibrosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IAutorService _autorService;
        private readonly ILogger<LibrosController> _logger;

        public LibrosController(ApplicationDbContext context, IWebHostEnvironment env, IAutorService autorService, ILogger<LibrosController> logger)
        {
            _context = context;
            _env = env;
            _autorService = autorService;
            _logger = logger;
        }

        private string UsuarioActual => User.Identity?.Name ?? "anónimo";

        public async Task<IActionResult> Index()
        {
            _logger.LogInformation("Inicio de consulta del listado de libros por {Usuario}", UsuarioActual);
            ViewBag.Autores = _autorService.ObtenerTodos();
            var libros = await _context.Libros.ToListAsync();
            _logger.LogInformation("Listado de libros consultado: {Cantidad} registros", libros.Count);
            return View(libros);
        }

        public async Task<IActionResult> Details(int id)
        {
            _logger.LogInformation("Inicio de consulta del detalle del libro {LibroId} por {Usuario}", id, UsuarioActual);
            var libro = await _context.Libros.FindAsync(id);
            if (libro == null)
            {
                _logger.LogWarning("No se encontró el libro {LibroId} al consultar su detalle", id);
                return NotFound();
            }
            ViewBag.Autor = _autorService.ObtenerPorId(libro.AutorId);
            _logger.LogInformation("Detalle del libro {LibroId} consultado correctamente", id);
            return View(libro);
        }

        [Authorize(Roles = RolesApp.Gestores)]
        public IActionResult Create()
        {
            _logger.LogInformation("Apertura del formulario de creación de libro por {Usuario}", UsuarioActual);
            ViewBag.Autores = _autorService.ObtenerTodos();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RolesApp.Gestores)]
        public async Task<IActionResult> Create(Libro model, IFormFile? Imagen)
        {
            _logger.LogInformation("Inicio de creación de libro por {Usuario}", UsuarioActual);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Creación de libro rechazada por datos no válidos ({Errores} errores de validación)", ModelState.ErrorCount);
                ViewBag.Autores = _autorService.ObtenerTodos();
                return View(model);
            }

            try
            {
                if (Imagen != null && Imagen.Length > 0)
                {
                    model.ImagenUrl = await GuardarImagen(Imagen);
                }

                _context.Libros.Add(model);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el libro {Titulo}", model.Titulo);
                throw;
            }

            _logger.LogInformation("Libro {LibroId} creado correctamente por {Usuario}", model.Id, UsuarioActual);
            TempData["Mensaje"] = $"\"{model.Titulo}\" se agregó correctamente.";
            return RedirectToAction("Index");
        }

        [Authorize(Roles = RolesApp.Gestores)]
        public async Task<IActionResult> Edit(int id)
        {
            _logger.LogInformation("Apertura del formulario de edición del libro {LibroId} por {Usuario}", id, UsuarioActual);
            var libro = await _context.Libros.FindAsync(id);
            if (libro == null)
            {
                _logger.LogWarning("No se encontró el libro {LibroId} para editarlo", id);
                return NotFound();
            }
            ViewBag.Autores = _autorService.ObtenerTodos();
            return View(libro);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RolesApp.Gestores)]
        public async Task<IActionResult> Edit(int id, Libro model, IFormFile? Imagen)
        {
            _logger.LogInformation("Inicio de edición del libro {LibroId} por {Usuario}", id, UsuarioActual);

            if (!ModelState.IsValid)
            {
                _logger.LogWarning("Edición del libro {LibroId} rechazada por datos no válidos ({Errores} errores de validación)", id, ModelState.ErrorCount);
                model.Id = id;
                ViewBag.Autores = _autorService.ObtenerTodos();
                return View(model);
            }

            var libro = await _context.Libros.FindAsync(id);
            if (libro == null)
            {
                _logger.LogWarning("No se encontró el libro {LibroId} al intentar actualizarlo", id);
                return NotFound();
            }

            try
            {
                libro.Titulo = model.Titulo;
                libro.Genero = model.Genero;
                libro.Anio = model.Anio;
                libro.Disponible = model.Disponible;
                libro.AutorId = model.AutorId;

                if (Imagen != null && Imagen.Length > 0)
                {
                    libro.ImagenUrl = await GuardarImagen(Imagen);
                }

                _context.Libros.Update(libro);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el libro {LibroId}", id);
                throw;
            }

            _logger.LogInformation("Libro {LibroId} actualizado correctamente por {Usuario}", id, UsuarioActual);
            TempData["Mensaje"] = $"\"{libro.Titulo}\" se actualizó correctamente.";
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = RolesApp.Gestores)]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation("Inicio de eliminación del libro {LibroId} por {Usuario}", id, UsuarioActual);
            var libro = await _context.Libros.FindAsync(id);

            if (libro == null)
            {
                _logger.LogWarning("No se encontró el libro {LibroId} al intentar eliminarlo", id);
                return RedirectToAction("Index");
            }

            try
            {
                _context.Libros.Remove(libro);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el libro {LibroId}", id);
                throw;
            }

            _logger.LogInformation("Libro {LibroId} eliminado correctamente por {Usuario}", id, UsuarioActual);
            TempData["Mensaje"] = $"\"{libro.Titulo}\" se eliminó correctamente.";
            return RedirectToAction("Index");
        }

        private async Task<string> GuardarImagen(IFormFile imagen)
        {
            var carpeta = Path.Combine(_env.WebRootPath, "images");
            if (!Directory.Exists(carpeta)) Directory.CreateDirectory(carpeta);

            var nombreArchivo = Guid.NewGuid() + Path.GetExtension(imagen.FileName);
            var ruta = Path.Combine(carpeta, nombreArchivo);

            using (var stream = new FileStream(ruta, FileMode.Create))
            {
                await imagen.CopyToAsync(stream);
            }

            return "/images/" + nombreArchivo;
        }
    }
}
