using System.ComponentModel.DataAnnotations;

namespace Biblioteca.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Ingresá tu usuario o correo.")]
        [Display(Name = "Usuario o correo")]
        public string UsuarioOCorreo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ingresá tu contraseña.")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; } = string.Empty;

        [Display(Name = "Recordarme")]
        public bool Recordarme { get; set; }

        public string? ReturnUrl { get; set; }
    }
}
