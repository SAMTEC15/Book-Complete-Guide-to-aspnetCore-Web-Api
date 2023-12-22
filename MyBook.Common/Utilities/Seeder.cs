using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using MyBook.Domain.Models;

namespace MyBook.Common.Utilities
{
    public class Seeder
    {
        public static void SeedRolesAndSuperAdmin(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var managerManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();

            // Seed roles
            if (!roleManager.RoleExistsAsync("SuperAdmin").Result)
            {
                var role = new IdentityRole("SuperAdmin");
                roleManager.CreateAsync(role).Wait();
            }

            if (!roleManager.RoleExistsAsync("Manager").Result)
            {
                var role = new IdentityRole("Manager");
                roleManager.CreateAsync(role).Wait();

            }
            if (!roleManager.RoleExistsAsync("User").Result)
            {
                var role = new IdentityRole("User");
                roleManager.CreateAsync(role).Wait();
            }

        }
    }
}
