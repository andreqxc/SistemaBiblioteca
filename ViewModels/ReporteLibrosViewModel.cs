using Biblioteca.Models;

namespace Biblioteca.ViewModels
{
    public record LibroReporteItem(int Id, string Titulo, string Autor, string Genero, int Anio, bool Disponible);

    public class ReporteLibrosViewModel
    {
        public List<LibroReporteItem> Libros { get; set; } = new();
        public List<Autor> Autores { get; set; } = new();
        public List<string> Generos { get; set; } = new();
        public int? AutorId { get; set; }
        public string? Genero { get; set; }
    }
}
