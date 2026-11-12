using Biblioteca.Data;
using Biblioteca.Services;
using Biblioteca.ViewModels;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Biblioteca.Controllers
{
    [Authorize]
    public class ReportesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAutorService _autorService;
        private readonly ILogger<ReportesController> _logger;

        public ReportesController(ApplicationDbContext context, IAutorService autorService, ILogger<ReportesController> logger)
        {
            _context = context;
            _autorService = autorService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int? autorId, string? genero)
        {
            var modelo = new ReporteLibrosViewModel
            {
                Libros = await ObtenerLibrosAsync(autorId, genero),
                Autores = _autorService.ObtenerTodos(),
                Generos = await _context.Libros.Select(l => l.Genero).Distinct().OrderBy(g => g).ToListAsync(),
                AutorId = autorId,
                Genero = genero
            };

            return View(modelo);
        }

        public async Task<IActionResult> LibrosPdf(int? autorId, string? genero)
        {
            var libros = await ObtenerLibrosAsync(autorId, genero);
            var filtros = DescribirFiltros(autorId, genero);
            var colorPrincipal = Colors.Brown.Darken3;

            var pdf = Document.Create(documento =>
            {
                documento.Page(pagina =>
                {
                    pagina.Size(PageSizes.A4);
                    pagina.Margin(30);
                    pagina.DefaultTextStyle(x => x.FontSize(10));

                    pagina.Header().Column(columna =>
                    {
                        columna.Item().Text("Reporte de Libros").FontSize(22).Bold().FontColor(colorPrincipal);
                        columna.Item().Text("Sistema Biblioteca Virtual").FontSize(12).FontColor(Colors.Grey.Darken1);
                        columna.Item().Text($"Fecha de generación: {DateTime.Now:dd/MM/yyyy HH:mm}");
                        columna.Item().Text($"Filtros: {filtros}");
                        columna.Item().PaddingTop(6).LineHorizontal(1).LineColor(colorPrincipal);
                    });

                    pagina.Content().PaddingVertical(10).Column(columna =>
                    {
                        columna.Item().Background(Colors.Brown.Lighten4).Padding(6)
                            .Text($"Total de libros incluidos en el reporte: {libros.Count}").Bold();

                        columna.Item().PaddingTop(10).Table(tabla =>
                        {
                            tabla.ColumnsDefinition(c =>
                            {
                                c.ConstantColumn(35);
                                c.RelativeColumn(3);
                                c.RelativeColumn(2.5f);
                                c.RelativeColumn(2);
                                c.ConstantColumn(40);
                                c.ConstantColumn(75);
                            });

                            tabla.Header(h =>
                            {
                                h.Cell().Element(Encabezado).Text("ID");
                                h.Cell().Element(Encabezado).Text("Libro");
                                h.Cell().Element(Encabezado).Text("Autor");
                                h.Cell().Element(Encabezado).Text("Categoría");
                                h.Cell().Element(Encabezado).Text("Año");
                                h.Cell().Element(Encabezado).Text("Disponible");
                            });

                            foreach (var libro in libros)
                            {
                                tabla.Cell().Element(Celda).Text(libro.Id.ToString());
                                tabla.Cell().Element(Celda).Text(libro.Titulo);
                                tabla.Cell().Element(Celda).Text(libro.Autor);
                                tabla.Cell().Element(Celda).Text(libro.Genero);
                                tabla.Cell().Element(Celda).Text(libro.Anio.ToString());
                                tabla.Cell().Element(Celda).Text(libro.Disponible ? "Sí" : "No");
                            }
                        });
                    });

                    pagina.Footer().AlignCenter().Text(texto =>
                    {
                        texto.Span("Página ");
                        texto.CurrentPageNumber();
                        texto.Span(" de ");
                        texto.TotalPages();
                    });
                });
            }).GeneratePdf();

            _logger.LogInformation("Reporte PDF generado con {Cantidad} libros", libros.Count);
            return File(pdf, "application/pdf", "ReporteLibros.pdf");
        }

        public async Task<IActionResult> LibrosExcel(int? autorId, string? genero)
        {
            var libros = await ObtenerLibrosAsync(autorId, genero);
            var filtros = DescribirFiltros(autorId, genero);

            using var libro = new XLWorkbook();
            var hoja = libro.Worksheets.Add("Reporte de Libros");

            hoja.Cell(1, 1).Value = "Reporte de Libros - Biblioteca Virtual";
            hoja.Cell(1, 1).Style.Font.Bold = true;
            hoja.Cell(1, 1).Style.Font.FontSize = 16;
            hoja.Cell(2, 1).Value = $"Fecha de generación: {DateTime.Now:dd/MM/yyyy HH:mm}";
            hoja.Cell(3, 1).Value = $"Filtros: {filtros}";
            hoja.Cell(4, 1).Value = $"Total de libros: {libros.Count}";
            hoja.Cell(4, 1).Style.Font.Bold = true;

            const int filaEncabezado = 6;
            var encabezados = new[] { "ID", "Libro", "Autor", "Categoría", "Año", "Disponible" };
            for (var i = 0; i < encabezados.Length; i++)
                hoja.Cell(filaEncabezado, i + 1).Value = encabezados[i];

            var rangoEncabezado = hoja.Range(filaEncabezado, 1, filaEncabezado, encabezados.Length);
            rangoEncabezado.Style.Font.Bold = true;
            rangoEncabezado.Style.Font.FontColor = XLColor.White;
            rangoEncabezado.Style.Fill.BackgroundColor = XLColor.FromHtml("#5B3A29");
            rangoEncabezado.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            var fila = filaEncabezado + 1;
            foreach (var item in libros)
            {
                hoja.Cell(fila, 1).Value = item.Id;
                hoja.Cell(fila, 2).Value = item.Titulo;
                hoja.Cell(fila, 3).Value = item.Autor;
                hoja.Cell(fila, 4).Value = item.Genero;
                hoja.Cell(fila, 5).Value = item.Anio;
                hoja.Cell(fila, 6).Value = item.Disponible ? "Sí" : "No";

                if (fila % 2 == 0)
                    hoja.Range(fila, 1, fila, encabezados.Length).Style.Fill.BackgroundColor = XLColor.FromHtml("#F5EDE6");

                fila++;
            }

            var rangoTabla = hoja.Range(filaEncabezado, 1, Math.Max(fila - 1, filaEncabezado), encabezados.Length);
            rangoTabla.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            rangoTabla.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            hoja.Range(filaEncabezado + 1, 1, Math.Max(fila - 1, filaEncabezado + 1), 1).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            hoja.Range(filaEncabezado + 1, 5, Math.Max(fila - 1, filaEncabezado + 1), 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            hoja.Columns().AdjustToContents(filaEncabezado, Math.Max(fila - 1, filaEncabezado));

            using var flujo = new MemoryStream();
            libro.SaveAs(flujo);

            _logger.LogInformation("Reporte Excel generado con {Cantidad} libros", libros.Count);
            return File(flujo.ToArray(),
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                "ReporteLibros.xlsx");
        }

        private async Task<List<LibroReporteItem>> ObtenerLibrosAsync(int? autorId, string? genero)
        {
            var consulta = _context.Libros.AsNoTracking().AsQueryable();

            if (autorId.HasValue)
                consulta = consulta.Where(l => l.AutorId == autorId.Value);

            if (!string.IsNullOrWhiteSpace(genero))
                consulta = consulta.Where(l => l.Genero == genero);

            var libros = await consulta.OrderBy(l => l.Titulo).ToListAsync();
            var autores = _autorService.ObtenerTodos();

            return libros.Select(l =>
            {
                var autor = autores.FirstOrDefault(a => a.Id == l.AutorId);
                var nombreAutor = autor != null ? $"{autor.Nombre} {autor.Apellido}" : "N/A";
                return new LibroReporteItem(l.Id, l.Titulo, nombreAutor, l.Genero, l.Anio, l.Disponible);
            }).ToList();
        }

        private string DescribirFiltros(int? autorId, string? genero)
        {
            var autor = autorId.HasValue ? _autorService.ObtenerPorId(autorId.Value) : null;
            var textoAutor = autor != null ? $"{autor.Nombre} {autor.Apellido}" : "Todos";
            var textoGenero = string.IsNullOrWhiteSpace(genero) ? "Todas" : genero;
            return $"Autor: {textoAutor} | Categoría: {textoGenero}";
        }

        private static IContainer Encabezado(IContainer contenedor) =>
            contenedor.DefaultTextStyle(x => x.SemiBold().FontColor(Colors.White))
                .Background(Colors.Brown.Darken3).Padding(5);

        private static IContainer Celda(IContainer contenedor) =>
            contenedor.BorderBottom(1).BorderColor(Colors.Grey.Lighten2).Padding(5);
    }
}
