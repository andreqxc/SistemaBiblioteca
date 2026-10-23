namespace Biblioteca.Data
{
    public static class RolesApp
    {
        public const string Admin = "Admin";
        public const string Bibliotecario = "Bibliotecario";
        public const string Usuario = "Usuario";

        public const string Gestores = Admin + "," + Bibliotecario;

        public static readonly string[] Todos = { Admin, Bibliotecario, Usuario };

        public static string? RolDe(System.Security.Claims.ClaimsPrincipal usuario) =>
            Todos.FirstOrDefault(usuario.IsInRole);
    }
}
