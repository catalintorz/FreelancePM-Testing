using FreelancePM.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FreelancePM.Pages
{
    [AllowAnonymous]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public IndexModel(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public string Greeting { get; set; } = "";
        public string? Username { get; set; }
        public bool IsAdmin { get; set; }

        // Admin metrics
        public int TotalUsers { get; set; }
        public int TotalActiveProjects { get; set; }

        // User metrics
        public int ClientsCount { get; set; }
        public int ProjectsCount { get; set; }
        public int ActiveTasksCount { get; set; }
        public int OverdueTasksCount { get; set; }

        public int PublicTotalUsers { get; set; }
        public int PublicTotalProjects { get; set; }
        public int PublicTotalTasks { get; set; }

        public async Task OnGetAsync()
        {
            // Statistici pentru landing page (no login)
            PublicTotalUsers = Math.Max(await _userManager.Users.CountAsync(), 1200);
            PublicTotalProjects = Math.Max(await _context.Projects.CountAsync(), 500);
            PublicTotalTasks = Math.Max(await _context.WorkTasks.CountAsync(), 3000);


            if (!User.Identity?.IsAuthenticated ?? true)
                return;

            Username = User.Identity?.Name?.Split('@')[0];
            var hour = DateTime.Now.Hour;
            Greeting = hour switch
            {
                >= 5 and < 12 => "Good morning",
                >= 12 and < 18 => "Good afternoon",
                _ => "Good evening"
            };

            IsAdmin = User.IsInRole("Admin");



            if (IsAdmin)
            {
                TotalUsers = await _userManager.Users.CountAsync();
                TotalActiveProjects = await _context.Projects.Where(p => p.Deadline >= DateTime.Today).CountAsync();
            }
            else
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                ClientsCount = await _context.Clients
                    .Where(c => c.UserId == userId)
                    .CountAsync();

                ProjectsCount = await _context.Projects
                    .Where(p => p.UserId == userId)
                    .CountAsync();

                ActiveTasksCount = await _context.WorkTasks
                    .Where(t => t.UserId == userId && t.StatusId != 3) // not Completed
                    .CountAsync();

                OverdueTasksCount = await _context.WorkTasks
                    .Where(t => t.UserId == userId && t.StatusId != 3 && t.Deadline < DateTime.Today)
                    .CountAsync();
            }            

        }
    }
}
