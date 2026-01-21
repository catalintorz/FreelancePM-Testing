using FreelancePM.Models;
using Microsoft.AspNetCore.Identity;

namespace FreelancePM.Data
{
    public class DbInitializer
    {
        public static void Seed(ApplicationDbContext context,UserManager<IdentityUser> userManager,
                                RoleManager<IdentityRole> roleManager)
        {
            context.Database.EnsureCreated();

            // Dacă există statusuri, nu mai inserăm
            if (context.Statuses.Any())
            {
                return;
            }

            var statuses = new List<Status>
            {
                new Status { Name = "To Do", Description = "Taskul nu a fost început" },
                new Status { Name = "In Progress", Description = "Taskul este în desfășurare" },
                new Status { Name = "Completed", Description = "Taskul a fost completat" }
            };

            context.Statuses.AddRange(statuses);
            context.SaveChanges();

            

            var adminRole = "Admin";
            if (!roleManager.RoleExistsAsync(adminRole).Result)
            {
                var role = new IdentityRole(adminRole);
                roleManager.CreateAsync(role).Wait();
            }
            
            var adminEmail = "admin@example.com";
            var adminUser = userManager.FindByEmailAsync(adminEmail).Result;
            if (adminUser == null)
            {
                adminUser = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                
                var result = userManager.CreateAsync(adminUser, "Admin123!").Result;
                if (result.Succeeded)
                {                    
                    userManager.AddToRoleAsync(adminUser, adminRole).Wait();
                }
            }

        }
    }
}
