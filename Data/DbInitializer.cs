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

            // SEEDS
            // Populare Statuses daca nu exista
            if (!context.Statuses.Any())
            {
                context.Statuses.AddRange(
                    new Status { Name = "To Do", Description = "Task has not been started yet." },
                    new Status { Name = "In Progress", Description = "Task is currently in progress." },
                    new Status { Name = "Completed", Description = "Task has been completed." }
                );
                context.SaveChanges();
            }


            // Rol si user de admin
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


            // SEED TEST DATA pentru user1@example.com
            const string userId = "14e890d7-e685-4064-82ef-70f82e96e498";
            const string userEmail = "user1@email.com";

            var user = userManager.FindByIdAsync(userId).Result;
            if (user == null)
            {
                user = new IdentityUser
                {
                    Id = userId,
                    UserName = userEmail,
                    Email = userEmail,
                    EmailConfirmed = true
                };
                userManager.CreateAsync(user, "User123!").Wait();
            }

            // Adaugam datele din seed doar daca nu are clienti introdusi
            if (context.Clients.Any(c => c.UserId == userId))
                return;

            var statuses = context.Statuses.ToList();
            var random = new Random();

            // Adaugare clienti      
            var clients = new List<Client>
            {
                new Client
                {
                    Name = "Acme Corporation",
                    Email = "contact@acme.com",
                    Phone = "+1 555-1234",
                    Company = "Acme Corp",
                    Notes = "Main long-term client. Lorem ipsum dolor sit amet.",
                    UserId = userId
                },
                new Client
                {
                    Name = "Blue Ocean Ltd",
                    Email = "info@blueocean.com",
                    Phone = "+1 555-5678",
                    Company = "Blue Ocean",
                    Notes = "Marketing and branding projects. Lorem ipsum dolor sit amet.",
                    UserId = userId
                },
                new Client
                {
                    Name = "Startup Hub",
                    Email = "hello@startuphub.io",
                    Phone = "+1 555-9999",
                    Company = "Startup Hub",
                    Notes = "Early-stage startup. Lorem ipsum dolor sit amet.",
                    UserId = userId
                }
            };

            context.Clients.AddRange(clients);
            context.SaveChanges();


            // Adaugare proiecte si taskuri            
            foreach (var client in clients)
            {
                int projectsCount = random.Next(1, 4);

                for (int i = 1; i <= projectsCount; i++)
                {
                    var project = new Project
                    {
                        Name = $"{client.Company} Project {i}",
                        Description = "Project description lorem ipsum dolor sit amet.",
                        StartDate = DateTime.Now.AddDays(-random.Next(10, 40)),
                        Deadline = DateTime.Now.AddDays(random.Next(10, 60)),
                        Budget = random.Next(1000, 10000),
                        ClientId = client.Id,
                        UserId = userId
                    };

                    context.Projects.Add(project);
                    context.SaveChanges();

                    int tasksCount = random.Next(1, 6);
                    bool allCompleted = random.Next(0, 2) == 1;

                    for (int t = 1; t <= tasksCount; t++)
                    {
                        var status = allCompleted
                            ? statuses.First(s => s.Name == "Completed")
                            : statuses[random.Next(statuses.Count)];

                        var task = new WorkTask
                        {
                            Title = $"Task {t} for {project.Name}",
                            Description = "Task details lorem ipsum dolor sit amet.",
                            Deadline = project.Deadline.AddDays(-random.Next(1, 10)),
                            StatusId = status.Id,
                            ProjectId = project.Id,
                            UserId = userId
                        };

                        context.WorkTasks.Add(task);
                    }

                    context.SaveChanges();
                }
            }


        }
    }
}
