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

namespace FreelancePM.Pages.Projects
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly FreelancePM.Data.ApplicationDbContext _context;

        public IndexModel(FreelancePM.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Project> Project { get;set; } = default!;


        // Filtru Clienti
        [BindProperty(SupportsGet = true)]
        public int? FilterClientId { get; set; }
        public SelectList ClientsList { get; set; } = default!;

        // Search dupa nume
        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }


        public async Task OnGetAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var query = _context.Projects
                .Where(p => p.UserId == userId)
                .Include(p => p.Client)
                .Include(p => p.WorkTasks)
                .ThenInclude(t => t.Status)
                .AsQueryable();

            // Filtrare pe client
            if (FilterClientId.HasValue)
            {
                query = query.Where(p => p.ClientId == FilterClientId.Value);
            }

            // Search dupa nume
            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                query = query.Where(p =>
                    p.Name.Contains(SearchTerm));
            }


            Project = await query.ToListAsync();

            // Dropdown clienti
            ClientsList = new SelectList(
                _context.Clients.Where(c => c.UserId == userId).ToList(),
                "Id",
                "Name",
                FilterClientId
            );
        }
    }
}
