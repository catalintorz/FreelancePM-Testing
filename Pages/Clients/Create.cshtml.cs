using FreelancePM.Data;
using FreelancePM.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FreelancePM.Pages.Clients
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly FreelancePM.Data.ApplicationDbContext _context;

        public CreateModel(FreelancePM.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            //ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return Page();
        }

        [BindProperty]
        public Client Client { get; set; } = default!;
        
        public async Task<IActionResult> OnPostAsync()
        {
            // Setare User
            Client.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!ModelState.IsValid)
            {
                
                return Page();
            }
            
            _context.Clients.Add(Client);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
