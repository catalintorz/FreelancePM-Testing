using FreelancePM.Data;
using FreelancePM.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FreelancePM.Pages.ProjectTasks
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly FreelancePM.Data.ApplicationDbContext _context;

        public IndexModel(FreelancePM.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        //Lista taskuri
        public IList<WorkTask> WorkTask { get;set; } = default!;

        // Dropdowns
        public SelectList ProjectsList { get; set; } = default!;
        public SelectList StatusesList { get; set; } = default!;

        // Filtre selectate
        [BindProperty(SupportsGet = true)]
        public int? FilterProjectId { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? FilterStatusId { get; set; }

        // Sortare
        [BindProperty(SupportsGet = true)]
        public string? SortOrder { get; set; }

        // Cautare
        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        public async Task OnGetAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Populare dropdowns
            ProjectsList = new SelectList(
                _context.Projects
                        .Where(p => p.UserId == userId)
                        .ToList(), "Id", "Name");

            StatusesList = new SelectList(
                _context.Statuses.ToList(), "Id", "Name");

            // Interogare taskuri
            var query = _context.WorkTasks
                .Where(t => t.UserId == userId)
                .Include(t => t.Project)
                .Include(t => t.Status)
                .AsQueryable();

            // Filters
            if (FilterProjectId.HasValue)
            {
                query = query.Where(t => t.ProjectId == FilterProjectId.Value);
            }

            if (FilterStatusId.HasValue)
            {
                query = query.Where(t => t.StatusId == FilterStatusId.Value);
            }

            // Cautare
            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                query = query.Where(t =>
                    t.Title.Contains(SearchTerm));
            }


            // Sortare
            if (SortOrder == "deadline_desc")
            {
                query = query.OrderByDescending(t => t.Deadline);
            }
            else
            {
                query = query.OrderBy(t => t.Deadline);
            }

            WorkTask = await query.ToListAsync();
        }

        public async Task<IActionResult> OnPostCompleteAsync(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var task = await _context.WorkTasks
                .Where(t => t.UserId == userId && t.Id == id)
                .FirstOrDefaultAsync();

            if (task == null)
                return NotFound();


            // Cautam statusul Completed
            var completedStatus = await _context.Statuses
                .FirstOrDefaultAsync(s => s.Name == "Completed");

            if (completedStatus == null)
                return BadRequest("Completed status not found.");

            task.StatusId = completedStatus.Id;

            await _context.SaveChangesAsync();

            return RedirectToPage();
        }

    }
}
