using Biblioteca.Data;
using Biblioteca.Models;
using Microsoft.AspNetCore.Mvc;

namespace Biblioteca.Controllers
{
    public class LibrosController : Controller
    {
        private const string Archivo = "libros.json";

        private static List<Libro> _libros = JsonStore.Cargar(Archivo, new List<Libro>
        {
            new Libro { Id = 1, Titulo = "Cien Años de Soledad", Genero = "Realismo Mágico", Anio = 1967, Disponible = true, AutorId = 1 },
            new Libro { Id = 2, Titulo = "La Casa de los Espíritus", Genero = "Realismo Mágico", Anio = 1982, Disponible = true, AutorId = 2 },
            new Libro { Id = 3, Titulo = "La Ciudad y los Perros", Genero = "Novela", Anio = 1963, Disponible = false, AutorId = 3 }
        });

        private readonly IWebHostEnvironment _env;

        public LibrosController(IWebHostEnvironment env)
        {
            _env = env;
        }

        public IActionResult Index()
        {
            return View(_libros);
        }

        public IActionResult Details(int id)
        {
            var libro = _libros.FirstOrDefault(l => l.Id == id);
            if (libro == null) return NotFound();
            return View(libro);
        }

        public IActionResult Create()
        {
            ViewBag.Autores = AutoresController.Autores;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Libro model, IFormFile? Imagen)
        {
            model.Id = _libros.Any() ? _libros.Max(l => l.Id) + 1 : 1;

            if (Imagen != null && Imagen.Length > 0)
            {
                model.ImagenUrl = await GuardarImagen(Imagen);
            }

            _libros.Add(model);
            JsonStore.Guardar(Archivo, _libros);
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            var libro = _libros.FirstOrDefault(l => l.Id == id);
            if (libro == null) return NotFound();
            ViewBag.Autores = AutoresController.Autores;
            return View(libro);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Libro model, IFormFile? Imagen)
        {
            var libro = _libros.FirstOrDefault(l => l.Id == id);
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

            JsonStore.Guardar(Archivo, _libros);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var libro = _libros.FirstOrDefault(l => l.Id == id);
            if (libro != null) _libros.Remove(libro);

            JsonStore.Guardar(Archivo, _libros);
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
