using System.ComponentModel.DataAnnotations;

namespace Biblioteca.Models
{
    public class Libro
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El título es obligatorio.")]
        [StringLength(150, ErrorMessage = "El título no puede superar los 150 caracteres.")]
        public string Titulo { get; set; }

        [Required(ErrorMessage = "El género es obligatorio.")]
        [StringLength(80, ErrorMessage = "El género no puede superar los 80 caracteres.")]
        public string Genero { get; set; }

        [Range(1450, 2100, ErrorMessage = "Ingresá un año válido entre 1450 y 2100.")]
        public int Anio { get; set; }

        public bool Disponible { get; set; }

        [Required(ErrorMessage = "Seleccioná un autor.")]
        public int AutorId { get; set; }

        public string? ImagenUrl { get; set; }
    }
}
