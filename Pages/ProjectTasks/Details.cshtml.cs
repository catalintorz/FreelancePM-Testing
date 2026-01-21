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
