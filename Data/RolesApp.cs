namespace Biblioteca.Data
{
    public static class RolesApp
    {
        public const string Administrador = "Administrador";
        public const string Bibliotecario = "Bibliotecario";
        public const string Lector = "Lector";

        public const string Gestores = Administrador + "," + Bibliotecario;

        public static readonly string[] Todos = { Administrador, Bibliotecario, Lector };

        public static string? RolDe(System.Security.Claims.ClaimsPrincipal usuario) =>
            Todos.FirstOrDefault(usuario.IsInRole);
    }
}
