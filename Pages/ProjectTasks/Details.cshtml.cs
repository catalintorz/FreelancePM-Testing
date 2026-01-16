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
    public class DetailsModel : PageModel
    {
        private readonly FreelancePM.Data.ApplicationDbContext _context;

        public DetailsModel(FreelancePM.Data.ApplicationDbContext context)
        {
            _context = context;
        }

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
    }
}
