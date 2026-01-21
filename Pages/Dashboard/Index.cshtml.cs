using FreelancePM.Data;
using FreelancePM.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FreelancePM.Pages.Dashboard
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        // KPI
        public int ClientsCount { get; set; }
        public int ProjectsCount { get; set; }
        public int TasksCount { get; set; }
        public int ActiveTasksCount { get; set; }

        // Charts
        public List<string> ProjectLabels { get; set; } = new();
        public List<int> ProjectValues { get; set; } = new();
        public List<string> ProjectColors { get; set; } = new();

        public List<string> StatusLabels { get; set; } = new();
        public List<int> StatusValues { get; set; } = new();
        public List<string> StatusColors { get; set; } = new();

        // Upcoming tasks (next 7 days + overdue)
        public List<WorkTask> UpcomingTasks { get; set; } = new();

        // User info
        public string? Username { get; set; }
        public string Greeting { get; set; } = "";

        public void OnGet()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Username = User.Identity?.Name?.Split('@')[0];
            var hour = DateTime.Now.Hour;
            Greeting = hour switch
            {
                >= 5 and < 12 => "Good morning",
                >= 12 and < 18 => "Good afternoon",
                _ => "Good evening"
            };

            var now = DateTime.Now;
            var next7Days = now.AddDays(7);

            // KPI
            ClientsCount = _context.Clients
                .Where(c => c.UserId == userId)
                .Count();

            ProjectsCount = _context.Projects
                .Where(p => p.UserId == userId)
                .Count();

            TasksCount = _context.WorkTasks
                .Where(t => t.UserId == userId)
                .Count();

            ActiveTasksCount = _context.WorkTasks
                .Where(t => t.UserId == userId && t.StatusId != 3) // 3 = Completed
                .Count();

            // Upcoming + Overdue tasks
            UpcomingTasks = _context.WorkTasks
                .Where(t => t.UserId == userId
                    && t.StatusId != 3             
                    && t.Deadline <= next7Days)    
                .Include(t => t.Project)
                .Include(t => t.Status)
                .OrderBy(t => t.Deadline)
                .ToList();

            // Chart 1 - Projects (Active vs Completed)
            var projects = _context.Projects
                .Where(p => p.UserId == userId)
                .ToList();

            ProjectLabels = new List<string> { "Active", "Completed" };
            ProjectValues = new List<int>
            {
                projects.Count(p => p.Deadline >= now),
                projects.Count(p => p.Deadline < now)
            };
            ProjectColors = new List<string> { "rgba(54, 162, 235, 0.8)", "rgba(75, 192, 192, 0.8)" };

            // Chart 2 - Tasks by Status
            var tasks = _context.WorkTasks
                .Where(t => t.UserId == userId)
                .Include(t => t.Status)
                .ToList();

            var taskStatusGroups = tasks
                .GroupBy(t => t.Status != null ? t.Status.Name : "Unknown")
                .Select(g => new { StatusName = g.Key, Count = g.Count() })
                .ToList();

            StatusLabels = taskStatusGroups.Select(x => x.StatusName).ToList();
            StatusValues = taskStatusGroups.Select(x => x.Count).ToList();

            // Colors
            var colors = new List<string>
            {
                "rgba(255, 99, 132, 0.8)",   // red-ish
                "rgba(255, 206, 86, 0.8)",   // yellow
                "rgba(54, 162, 235, 0.8)",   // blue
                "rgba(75, 192, 192, 0.8)",   // green
                "rgba(153, 102, 255, 0.8)",  // purple
                "rgba(255, 159, 64, 0.8)"    // orange
            };

            StatusColors = new List<string>();
            for (int i = 0; i < StatusLabels.Count; i++)
            {
                if (StatusLabels[i].ToLower() == "completed") // daca statusul e Completed
                    StatusColors.Add("rgba(75, 192, 192, 0.8)"); // verde
                else
                    StatusColors.Add(colors[i % colors.Count]);
            }
        }
    }

}