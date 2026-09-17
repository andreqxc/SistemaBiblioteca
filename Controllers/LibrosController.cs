using Biblioteca.Data;
using Biblioteca.Models;
using Biblioteca.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Biblioteca.Controllers
{
    public class LibrosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IAutorService _autorService;

        public LibrosController(ApplicationDbContext context, IWebHostEnvironment env, IAutorService autorService)
        {
            _context = context;
            _env = env;
            _autorService = autorService;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.Autores = _autorService.ObtenerTodos();
            var libros = await _context.Libros.ToListAsync();
            return View(libros);
        }

        public async Task<IActionResult> Details(int id)
        {
            var libro = await _context.Libros.FirstOrDefaultAsync(l => l.Id == id);
            if (libro == null) return NotFound();
            ViewBag.Autor = _autorService.ObtenerPorId(libro.AutorId);
            return View(libro);
        }

        public IActionResult Create()
        {
            ViewBag.Autores = _autorService.ObtenerTodos();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Libro model, IFormFile? Imagen)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Autores = _autorService.ObtenerTodos();
                return View(model);
            }

            if (Imagen != null && Imagen.Length > 0)
            {
                model.ImagenUrl = await GuardarImagen(Imagen);
            }

            _context.Libros.Add(model);
            await _context.SaveChangesAsync();
            TempData["Mensaje"] = $"\"{model.Titulo}\" se agregó correctamente.";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            var libro = await _context.Libros.FirstOrDefaultAsync(l => l.Id == id);
            if (libro == null) return NotFound();
            ViewBag.Autores = _autorService.ObtenerTodos();
            return View(libro);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Libro model, IFormFile? Imagen)
        {
            if (!ModelState.IsValid)
            {
                model.Id = id;
                ViewBag.Autores = _autorService.ObtenerTodos();
                return View(model);
            }

            var libro = await _context.Libros.FirstOrDefaultAsync(l => l.Id == id);
            if (libro == null) return NotFound();

            libro.Titulo = model.Titulo;
            libro.Genero = model.Genero;
            libro.Anio = model.Anio;
            libro.Disponible = model.Disponible;
            libro.AutorId = model.AutorId;

            if (Imagen != null && Imagen.Length > 0)
            {
                libro.ImagenUrl = await GuardarImagen(Imagen);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var libro = await _context.Libros.FirstOrDefaultAsync(l => l.Id == id);
            if (libro != null)
            {
                _context.Libros.Remove(libro);
                await _context.SaveChangesAsync();
            }
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
