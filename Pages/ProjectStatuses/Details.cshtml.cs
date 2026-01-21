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

namespace FreelancePM.Pages.ProjectStatuses
{
    [Authorize(Roles = "Admin")]
    public class DetailsModel : PageModel
    {
        private readonly FreelancePM.Data.ApplicationDbContext _context;

        public DetailsModel(FreelancePM.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public Status Status { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var status = await _context.Statuses.FirstOrDefaultAsync(m => m.Id == id);

            if (status is not null)
            {
                Status = status;

                return Page();
            }

            return NotFound();
        }
    }
}
