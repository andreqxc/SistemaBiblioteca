namespace Biblioteca.Models
{
    public class Libro
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Genero { get; set; }
        public int Anio { get; set; }
        public bool Disponible { get; set; }
        public int AutorId { get; set; }
        public string? ImagenUrl { get; set; }
    }
}
