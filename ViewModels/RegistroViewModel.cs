using System.ComponentModel.DataAnnotations;

namespace Biblioteca.ViewModels
{
    public class RegistroViewModel
    {
        [Required(ErrorMessage = "Ingresá un nombre de usuario.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "El usuario debe tener entre 3 y 50 caracteres.")]
        [RegularExpression(@"^[a-zA-Z0-9._-]+$", ErrorMessage = "Usá solo letras, números, punto, guion o guion bajo.")]
        [Display(Name = "Nombre de usuario")]
        public string NombreUsuario { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ingresá tu correo electrónico.")]
        [EmailAddress(ErrorMessage = "Ingresá un correo válido.")]
        [Display(Name = "Correo electrónico")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ingresá una contraseña.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Confirmá tu contraseña.")]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Las contraseñas no coinciden.")]
        [Display(Name = "Confirmar contraseña")]
        public string ConfirmarPassword { get; set; } = string.Empty;
    }
}
