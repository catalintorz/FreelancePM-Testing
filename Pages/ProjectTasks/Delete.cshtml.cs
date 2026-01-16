using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FreelancePM.Data;
using FreelancePM.Models;

namespace FreelancePM.Pages.ProjectTasks
{
    public class DeleteModel : PageModel
    {
        private readonly FreelancePM.Data.ApplicationDbContext _context;

        public DeleteModel(FreelancePM.Data.ApplicationDbContext context)
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

            var worktask = await _context.WorkTasks.FirstOrDefaultAsync(m => m.Id == id);

            if (worktask is not null)
            {
                WorkTask = worktask;

                return Page();
            }

            return NotFound();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var worktask = await _context.WorkTasks.FindAsync(id);
            if (worktask != null)
            {
                WorkTask = worktask;
                _context.WorkTasks.Remove(WorkTask);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
