namespace Biblioteca.ViewModels
{
    public class EstadoPaginaViewModel
    {
        public string Icono { get; set; } = "⚠️";
        public string Titulo { get; set; } = string.Empty;
        public string Mensaje { get; set; } = string.Empty;
        public string? Sugerencia { get; set; }
        public string Variante { get; set; } = "error";
        public string? Usuario { get; set; }
        public string? Rol { get; set; }
        public string? Referencia { get; set; }
    }
}
