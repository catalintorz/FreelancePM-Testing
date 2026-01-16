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

        // Statisticile
        public int ClientsCount { get; set; }
        public int ProjectsCount { get; set; }
        public int TasksCount { get; set; }
        public int ActiveTasksCount { get; set; }

        // Doughnut chart – Taskuri pe Status
        public List<string> StatusLabels { get; set; } = new();
        public List<int> StatusValues { get; set; } = new();
        public List<string> StatusColors { get; set; } = new();

        // Bar chart – Taskuri pe Proiect
        public List<string> ProjectLabels { get; set; } = new();
        public List<int> ProjectValues { get; set; } = new();
        public List<string> ProjectColors { get; set; } = new();

        // Taskuri cu deadline viitor (7 zile)
        public List<WorkTask> UpcomingTasks { get; set; } = new();

        // Afisare Username
        public string? Username { get; set; }
        // Greeting
        public string Greeting { get; set; } = "";



        public void OnGet()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            // Greeting Username
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

            ClientsCount = _context.Clients
                .Where(c => c.UserId == userId)
                .Count();

            ProjectsCount = _context.Projects
                .Where(p => p.UserId == userId)
                .Count();

            TasksCount = _context.WorkTasks
                .Where(t => t.UserId == userId)
                .Count();

            // Active = taskuri care nu sunt Finalizate
            ActiveTasksCount = _context.WorkTasks
                .Where(t => t.UserId == userId && t.StatusId != 3) // presupunem 3 = Finalizat
                .Count();


            // Taskuri cu deadline viitor
            UpcomingTasks = _context.WorkTasks
                .Where(t => t.UserId == userId && t.Deadline >= now && t.Deadline <= next7Days)
                .Include(t => t.Project)
                .Include(t => t.Status)
                .OrderBy(t => t.Deadline)
                .ToList();

            // Doughnut chart: Taskuri pe Status
            var taskStatus = _context.WorkTasks
                .Where(t => t.UserId == userId)
                .GroupBy(t => t.Status)
                .Select(g => new
                {
                    StatusName = g.Key != null ? g.Key.Name : "Necunoscut",
                    Count = g.Count()
                })
                .ToList();

            StatusLabels = taskStatus.Select(x => x.StatusName).ToList();
            StatusValues = taskStatus.Select(x => x.Count).ToList();

            var statusColors = new List<string>
            {
                "rgba(54, 162, 235, 0.7)",
                "rgba(255, 206, 86, 0.7)",
                "rgba(75, 192, 192, 0.7)",
                "rgba(255, 99, 132, 0.7)",
                "rgba(153, 102, 255, 0.7)",
                "rgba(255, 159, 64, 0.7)"
            };
            StatusColors = new List<string>();
            for (int i = 0; i < StatusLabels.Count; i++)
                StatusColors.Add(statusColors[i % statusColors.Count]);

            // Bar chart: Taskuri pe Proiect
            var tasksByProject = _context.WorkTasks
                .Where(t => t.UserId == userId)
                .GroupBy(t => t.Project)
                .Select(g => new
                {
                    ProjectName = g.Key != null ? g.Key.Name : "Necunoscut",
                    Count = g.Count()
                })
                .ToList();

            ProjectLabels = tasksByProject.Select(x => x.ProjectName).ToList();
            ProjectValues = tasksByProject.Select(x => x.Count).ToList();

            var projectColors = new List<string>
            {
                "rgba(255, 99, 132, 0.7)",
                "rgba(54, 162, 235, 0.7)",
                "rgba(255, 206, 86, 0.7)",
                "rgba(75, 192, 192, 0.7)",
                "rgba(153, 102, 255, 0.7)",
                "rgba(255, 159, 64, 0.7)"
            };
            ProjectColors = new List<string>();
            for (int i = 0; i < ProjectLabels.Count; i++)
                ProjectColors.Add(projectColors[i % projectColors.Count]);
        }
    }
}
