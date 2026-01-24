using FreelancePM.Data;
using FreelancePM.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FreelancePM.Pages.Clients
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;        

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Client> Client { get; set; } = default!;

        // Cautare
        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }
       
        public async Task OnGetAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var query = _context.Clients
               .Where(c => c.UserId == userId)
               .Include(c => c.Projects)
               .AsQueryable();

            // Cautare dupa Name sau Company
            if (!string.IsNullOrWhiteSpace(SearchTerm))
            {
                query = query.Where(c =>
                    c.Name.Contains(SearchTerm) || (c.Company != null && c.Company.Contains(SearchTerm))
                );
            }

            Client = await query.ToListAsync();
        }
    }
}
