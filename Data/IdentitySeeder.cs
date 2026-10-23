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

            await RenombrarRolAsync(roleManager, "Administrador", RolesApp.Admin);
            await RenombrarRolAsync(roleManager, "Lector", RolesApp.Usuario);

            foreach (var rol in RolesApp.Todos)
            {
                if (!await roleManager.RoleExistsAsync(rol))
                    await roleManager.CreateAsync(new IdentityRole(rol));
            }

            await CrearUsuarioAsync(userManager, "admin", "admin@biblioteca.com", "Admin123", RolesApp.Admin);
            await CrearUsuarioAsync(userManager, "bibliotecario", "bibliotecario@biblioteca.com", "Biblio123", RolesApp.Bibliotecario);
            await CrearUsuarioAsync(userManager, "lector", "lector@biblioteca.com", "Lector123", RolesApp.Usuario);

            var usuarios = await userManager.Users.ToListAsync();
            foreach (var usuario in usuarios)
            {
                var roles = await userManager.GetRolesAsync(usuario);
                if (roles.Count == 0)
                    await userManager.AddToRoleAsync(usuario, RolesApp.Usuario);
            }
        }

        private static async Task RenombrarRolAsync(RoleManager<IdentityRole> roleManager, string nombreAnterior, string nombreNuevo)
        {
            if (await roleManager.RoleExistsAsync(nombreNuevo)) return;

            var rol = await roleManager.FindByNameAsync(nombreAnterior);
            if (rol == null) return;

            await roleManager.SetRoleNameAsync(rol, nombreNuevo);
            await roleManager.UpdateAsync(rol);
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
