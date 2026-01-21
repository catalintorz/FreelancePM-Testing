using FreelancePM.Data;
using FreelancePM.Models;
using FreelancePM.Resources; // <-- aici e SharedResources
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization; // <-- localizer
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
        private readonly IStringLocalizer<SharedResources> _localizer; // <-- campul pentru localizare

        // Constructorul injecteaza atat DB context cat si localizer
        public IndexModel(ApplicationDbContext context, IStringLocalizer<SharedResources> localizer)
        {
            _context = context;
            _localizer = localizer;
        }

        public IList<Client> Client { get; set; } = default!;

        // Cautare
        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        // Exemple de texte localizate in cod
        public string ClientsText { get; private set; } = string.Empty;
        public string CreateClientText { get; private set; } = string.Empty;

        public async Task OnGetAsync()
        {
            // Localizare: preiau textele pentru afisare in Razor
            ClientsText = _localizer["Clients"];
            CreateClientText = _localizer["CreateClient"];

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
