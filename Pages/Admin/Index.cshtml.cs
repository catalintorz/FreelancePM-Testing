using FreelancePM.Data;
using FreelancePM.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace FreelancePM.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public IndexModel(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // KPI
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int TotalProjects { get; set; }
        public int ActiveProjects { get; set; }

        // Chart - projects per month
        public List<string> ProjectChartLabels { get; set; } = new();
        public List<int> ProjectChartValues { get; set; } = new();

        // Last users (Identity standard)
        public List<IdentityUser> LastUsers { get; set; } = new();

        // Recent projects
        public List<Project> RecentProjects { get; set; } = new();

        public async Task OnGetAsync()
        {
            // USERS
            TotalUsers = _userManager.Users.Count();

            ActiveUsers = await _context.WorkTasks
                .Select(t => t.UserId)
                .Distinct()
                .CountAsync();

            // PROJECTS
            TotalProjects = await _context.Projects.CountAsync();

            ActiveProjects = await _context.Projects
                .Where(p => p.Deadline >= DateTime.Today)
                .CountAsync();

            // LAST USERS (best possible with Identity standard)
            LastUsers = await _userManager.Users
                .OrderByDescending(u => u.Id)
                .Take(5)
                .ToListAsync();

            // RECENT PROJECTS
            RecentProjects = await _context.Projects
                .Include(p => p.Client)
                .OrderByDescending(p => p.StartDate)
                .Take(5)
                .ToListAsync();

            // PROJECT CHART - last 6 months
            var sixMonthsAgo = DateTime.Today.AddMonths(-5);

            var projectsByMonthRaw = await _context.Projects
                .Where(p => p.StartDate >= sixMonthsAgo)
                .GroupBy(p => new { p.StartDate.Year, p.StartDate.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    Count = g.Count()
                })
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
                .ToListAsync();


            ProjectChartLabels = projectsByMonthRaw.Select(x => $"{x.Month:D2}/{x.Year}").ToList();

            ProjectChartValues = projectsByMonthRaw.Select(x => x.Count).ToList();

        }
    }
}
