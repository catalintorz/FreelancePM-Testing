using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using FreelancePM.Data;
using FreelancePM.Models;
using Microsoft.AspNetCore.Authorization;

namespace FreelancePM.Pages.ProjectStatuses
{
    [Authorize(Roles = "Admin")]
    public class IndexModel : PageModel
    {
        private readonly FreelancePM.Data.ApplicationDbContext _context;

        public IndexModel(FreelancePM.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Status> Status { get;set; } = default!;

        public async Task OnGetAsync()
        {
            Status = await _context.Statuses.ToListAsync();
        }
    }
}
