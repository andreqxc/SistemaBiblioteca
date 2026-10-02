using Microsoft.AspNetCore.Identity;

namespace Biblioteca.Services
{
    public class IdentityErrorDescriberEs : IdentityErrorDescriber
    {
        public override IdentityError DefaultError() =>
            new() { Code = nameof(DefaultError), Description = "Ocurrió un error inesperado." };

        public override IdentityError DuplicateUserName(string userName) =>
            new() { Code = nameof(DuplicateUserName), Description = $"El usuario '{userName}' ya está registrado." };

        public override IdentityError DuplicateEmail(string email) =>
            new() { Code = nameof(DuplicateEmail), Description = $"El correo '{email}' ya está registrado." };

        public override IdentityError InvalidUserName(string? userName) =>
            new() { Code = nameof(InvalidUserName), Description = "El nombre de usuario contiene caracteres no permitidos." };

        public override IdentityError InvalidEmail(string? email) =>
            new() { Code = nameof(InvalidEmail), Description = "El correo electrónico no es válido." };

        public override IdentityError PasswordTooShort(int length) =>
            new() { Code = nameof(PasswordTooShort), Description = $"La contraseña debe tener al menos {length} caracteres." };

        public override IdentityError PasswordRequiresDigit() =>
            new() { Code = nameof(PasswordRequiresDigit), Description = "La contraseña debe incluir al menos un número." };

        public override IdentityError PasswordRequiresLower() =>
            new() { Code = nameof(PasswordRequiresLower), Description = "La contraseña debe incluir al menos una minúscula." };

        public override IdentityError PasswordRequiresUpper() =>
            new() { Code = nameof(PasswordRequiresUpper), Description = "La contraseña debe incluir al menos una mayúscula." };

        public override IdentityError PasswordRequiresNonAlphanumeric() =>
            new() { Code = nameof(PasswordRequiresNonAlphanumeric), Description = "La contraseña debe incluir al menos un símbolo." };

        public override IdentityError PasswordRequiresUniqueChars(int uniqueChars) =>
            new() { Code = nameof(PasswordRequiresUniqueChars), Description = $"La contraseña debe tener al menos {uniqueChars} caracteres distintos." };
    }
}
