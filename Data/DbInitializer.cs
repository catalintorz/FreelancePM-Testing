using FreelancePM.Models;
using Microsoft.AspNetCore.Identity;

namespace FreelancePM.Data
{
    public class DbInitializer
    {
        public static void Seed(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

            // Dacă există statusuri, nu mai inserăm
            if (context.Statuses.Any())
            {
                return;
            }

            var statuses = new List<Status>
            {
                new Status { Name = "De făcut", Description = "Taskul nu a fost început" },
                new Status { Name = "În lucru", Description = "Taskul este în desfășurare" },
                new Status { Name = "Finalizat", Description = "Taskul a fost completat" }
            };

            context.Statuses.AddRange(statuses);
            context.SaveChanges();
        }
    }
}
