using FreelancePM.Data;
using FreelancePM.Models;
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
    public class EditModel : PageModel
    {
        private readonly FreelancePM.Data.ApplicationDbContext _context;

        public EditModel(FreelancePM.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public WorkTask WorkTask { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var worktask =  await _context.WorkTasks.FirstOrDefaultAsync(m => m.Id == id);
            if (worktask == null)
            {
                return NotFound();
            }

            WorkTask = worktask;
            PopulateDropdowns();

            return Page();
        }

        
        public async Task<IActionResult> OnPostAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            WorkTask.UserId = userId;


            if (!ModelState.IsValid)
            {
                PopulateDropdowns();
                return Page();
            }

            _context.Attach(WorkTask).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WorkTaskExists(WorkTask.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool WorkTaskExists(int id)
        {
            return _context.WorkTasks.Any(e => e.Id == id);
        }

        private void PopulateDropdowns()
        {

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            ViewData["ProjectId"] = new SelectList(
                _context.Projects.Where(p => p.UserId == userId),
                "Id", "Name");

            ViewData["StatusId"] = new SelectList(
                _context.Statuses.ToList(),
                "Id", "Name");
        }
    }
}
