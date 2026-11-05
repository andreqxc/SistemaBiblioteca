using System.ComponentModel.DataAnnotations;
using Biblioteca.Controllers;
using Biblioteca.Data;
using Biblioteca.Models;
using Biblioteca.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Biblioteca.Tests;

public class LibrosControllerTests
{
    private readonly ApplicationDbContext _context;
    private readonly Mock<IAutorService> _autores;
    private readonly Mock<ILogger<LibrosController>> _logger;
    private readonly LibrosController _controller;

    public LibrosControllerTests()
    {
        var opciones = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context = new ApplicationDbContext(opciones);

        _autores = new Mock<IAutorService>();
        _autores.Setup(a => a.ObtenerTodos()).Returns(new List<Autor>
        {
            new Autor { Id = 1, Nombre = "Gabriel", Apellido = "García Márquez" }
        });
        _autores.Setup(a => a.ObtenerPorId(1)).Returns(new Autor { Id = 1, Nombre = "Gabriel", Apellido = "García Márquez" });

        _logger = new Mock<ILogger<LibrosController>>();

        var httpContext = new DefaultHttpContext();
        _controller = new LibrosController(_context, Mock.Of<IWebHostEnvironment>(), _autores.Object, _logger.Object)
        {
            ControllerContext = new ControllerContext { HttpContext = httpContext },
            TempData = new TempDataDictionary(httpContext, Mock.Of<ITempDataProvider>())
        };
    }

    private Libro SembrarLibro(string titulo = "Cien años de soledad")
    {
        var libro = new Libro { Titulo = titulo, Genero = "Novela", Anio = 1967, Disponible = true, AutorId = 1 };
        _context.Libros.Add(libro);
        _context.SaveChanges();
        return libro;
    }

    private void VerificarLog(LogLevel nivel, Times veces)
    {
        _logger.Verify(l => l.Log(
            nivel,
            It.IsAny<EventId>(),
            It.IsAny<It.IsAnyType>(),
            It.IsAny<Exception?>(),
            (Func<It.IsAnyType, Exception?, string>)It.IsAny<object>()), veces);
    }

    [Fact]
    public async Task Index_ConLibros_DevuelveTodosLosRegistros()
    {
        SembrarLibro("Cien años de soledad");
        SembrarLibro("La casa de los espíritus");

        var resultado = await _controller.Index();

        var vista = Assert.IsType<ViewResult>(resultado);
        var modelo = Assert.IsAssignableFrom<List<Libro>>(vista.Model);
        Assert.Equal(2, modelo.Count);
        _autores.Verify(a => a.ObtenerTodos(), Times.Once);
    }

    [Fact]
    public async Task Index_SinLibros_DevuelveListaVacia()
    {
        var resultado = await _controller.Index();

        var vista = Assert.IsType<ViewResult>(resultado);
        var modelo = Assert.IsAssignableFrom<List<Libro>>(vista.Model);
        Assert.Empty(modelo);
    }

    [Fact]
    public async Task Details_LibroExistente_DevuelveVistaConElLibro()
    {
        var libro = SembrarLibro();

        var resultado = await _controller.Details(libro.Id);

        var vista = Assert.IsType<ViewResult>(resultado);
        var modelo = Assert.IsType<Libro>(vista.Model);
        Assert.NotNull(modelo);
        Assert.Equal("Cien años de soledad", modelo.Titulo);
        _autores.Verify(a => a.ObtenerPorId(1), Times.Once);
    }

    [Fact]
    public async Task Details_LibroInexistente_DevuelveNotFoundYRegistraAdvertencia()
    {
        var resultado = await _controller.Details(999);

        Assert.IsType<NotFoundResult>(resultado);
        _autores.Verify(a => a.ObtenerPorId(It.IsAny<int>()), Times.Never);
        VerificarLog(LogLevel.Warning, Times.Once());
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(999)]
    public async Task Details_IdsInvalidosOInexistentes_DevuelveNotFound(int id)
    {
        var resultado = await _controller.Details(id);

        Assert.IsType<NotFoundResult>(resultado);
    }

    [Fact]
    public async Task Create_ModeloValido_GuardaElLibroYRedirigeAIndex()
    {
        var nuevo = new Libro { Titulo = "Rayuela", Genero = "Novela", Anio = 1963, Disponible = true, AutorId = 1 };

        var resultado = await _controller.Create(nuevo, null);

        var redireccion = Assert.IsType<RedirectToActionResult>(resultado);
        Assert.Equal("Index", redireccion.ActionName);
        var guardado = Assert.Single(_context.Libros);
        Assert.Equal("Rayuela", guardado.Titulo);
        Assert.NotNull(_controller.TempData["Mensaje"]);
    }

    [Fact]
    public async Task Create_ModeloInvalido_NoGuardaYDevuelveLaVista()
    {
        var invalido = new Libro { Titulo = "", Genero = "Novela", Anio = 1963, AutorId = 1 };
        _controller.ModelState.AddModelError("Titulo", "El título es obligatorio.");

        var resultado = await _controller.Create(invalido, null);

        var vista = Assert.IsType<ViewResult>(resultado);
        Assert.Same(invalido, vista.Model);
        Assert.Empty(_context.Libros);
        VerificarLog(LogLevel.Warning, Times.Once());
    }

    [Fact]
    public async Task Edit_ModeloValido_ActualizaElLibroYRedirige()
    {
        var libro = SembrarLibro();
        var cambios = new Libro { Titulo = "Título editado", Genero = "Ensayo", Anio = 2000, Disponible = false, AutorId = 1 };

        var resultado = await _controller.Edit(libro.Id, cambios, null);

        Assert.IsType<RedirectToActionResult>(resultado);
        var actualizado = await _context.Libros.FindAsync(libro.Id);
        Assert.NotNull(actualizado);
        Assert.Equal("Título editado", actualizado!.Titulo);
        Assert.Equal("Ensayo", actualizado.Genero);
        Assert.Equal(2000, actualizado.Anio);
        Assert.False(actualizado.Disponible);
    }

    [Fact]
    public async Task Edit_LibroInexistente_DevuelveNotFound()
    {
        var cambios = new Libro { Titulo = "X", Genero = "Y", Anio = 2000, AutorId = 1 };

        var resultado = await _controller.Edit(999, cambios, null);

        Assert.IsType<NotFoundResult>(resultado);
    }

    [Fact]
    public async Task Edit_ModeloInvalido_DevuelveLaVistaSinModificarElLibro()
    {
        var libro = SembrarLibro();
        var cambios = new Libro { Titulo = "", Genero = "Y", Anio = 2000, AutorId = 1 };
        _controller.ModelState.AddModelError("Titulo", "El título es obligatorio.");

        var resultado = await _controller.Edit(libro.Id, cambios, null);

        var vista = Assert.IsType<ViewResult>(resultado);
        Assert.Equal(libro.Id, Assert.IsType<Libro>(vista.Model).Id);
        var original = await _context.Libros.FindAsync(libro.Id);
        Assert.Equal("Cien años de soledad", original!.Titulo);
    }

    [Fact]
    public async Task Delete_LibroExistente_LoEliminaYRedirige()
    {
        var libro = SembrarLibro();

        var resultado = await _controller.Delete(libro.Id);

        var redireccion = Assert.IsType<RedirectToActionResult>(resultado);
        Assert.Equal("Index", redireccion.ActionName);
        Assert.Empty(_context.Libros);
    }

    [Fact]
    public async Task Delete_LibroInexistente_RedirigeSinEliminarNada()
    {
        SembrarLibro();

        var resultado = await _controller.Delete(999);

        Assert.IsType<RedirectToActionResult>(resultado);
        Assert.Single(_context.Libros);
        VerificarLog(LogLevel.Warning, Times.Once());
    }

    [Theory]
    [InlineData(1449, false)]
    [InlineData(1450, true)]
    [InlineData(2100, true)]
    [InlineData(2101, false)]
    public void Libro_ValidaElRangoDelAnio(int anio, bool esperado)
    {
        var libro = new Libro { Titulo = "T", Genero = "G", Anio = anio, AutorId = 1 };
        var resultados = new List<ValidationResult>();

        var valido = Validator.TryValidateObject(libro, new ValidationContext(libro), resultados, true);

        Assert.Equal(esperado, valido);
    }

    [Fact]
    public void Libro_SinTituloNiGenero_NoEsValido()
    {
        var libro = new Libro { Anio = 2000, AutorId = 1 };
        var resultados = new List<ValidationResult>();

        var valido = Validator.TryValidateObject(libro, new ValidationContext(libro), resultados, true);

        Assert.False(valido);
        Assert.Equal(2, resultados.Count);
    }
}
