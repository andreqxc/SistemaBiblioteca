namespace Biblioteca.Models
{
    public class ErrorViewModel
    {
        public string? RequestId { get; set; }

        public int? Codigo { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
