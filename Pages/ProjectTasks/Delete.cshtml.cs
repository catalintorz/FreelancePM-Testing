using FreelancePM.Data;
using FreelancePM.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FreelancePM.Pages.ProjectTasks
{
    [Authorize]
    public class DeleteModel : PageModel
    {        
        private readonly FreelancePM.Data.ApplicationDbContext _context;

        public DeleteModel(FreelancePM.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public WorkTask WorkTask { get; set; } = default!;

        public ApplicationDbContext Context => _context;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var worktask = await Context.WorkTasks.FirstOrDefaultAsync(m => m.Id == id);

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

            var worktask = await Context.WorkTasks.FindAsync(id);
            if (worktask != null)
            {
                WorkTask = worktask;
                Context.WorkTasks.Remove(WorkTask);
                await Context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
