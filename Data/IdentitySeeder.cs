using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Biblioteca.Data
{
    public static class IdentitySeeder
    {
        public static async Task SembrarAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

            foreach (var rol in RolesApp.Todos)
            {
                if (!await roleManager.RoleExistsAsync(rol))
                    await roleManager.CreateAsync(new IdentityRole(rol));
            }

            await CrearUsuarioAsync(userManager, "admin", "admin@biblioteca.com", "Admin123", RolesApp.Administrador);
            await CrearUsuarioAsync(userManager, "bibliotecario", "bibliotecario@biblioteca.com", "Biblio123", RolesApp.Bibliotecario);
            await CrearUsuarioAsync(userManager, "lector", "lector@biblioteca.com", "Lector123", RolesApp.Lector);

            var usuarios = await userManager.Users.ToListAsync();
            foreach (var usuario in usuarios)
            {
                var roles = await userManager.GetRolesAsync(usuario);
                if (roles.Count == 0)
                    await userManager.AddToRoleAsync(usuario, RolesApp.Lector);
            }
        }

        private static async Task CrearUsuarioAsync(UserManager<IdentityUser> userManager, string nombre, string email, string password, string rol)
        {
            var usuario = await userManager.FindByEmailAsync(email) ?? await userManager.FindByNameAsync(nombre);

            if (usuario == null)
            {
                usuario = new IdentityUser { UserName = nombre, Email = email };
                var resultado = await userManager.CreateAsync(usuario, password);
                if (!resultado.Succeeded) return;
            }

            var rolesActuales = await userManager.GetRolesAsync(usuario);
            if (rolesActuales.Count == 0)
                await userManager.AddToRoleAsync(usuario, rol);
        }
    }
}
