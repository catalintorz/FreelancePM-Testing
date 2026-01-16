using FreelancePM.Data;
using FreelancePM.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FreelancePM.Pages.Clients
{
    [Authorize]
    public class EditModel : PageModel
    {
        private readonly FreelancePM.Data.ApplicationDbContext _context;

        public EditModel(FreelancePM.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Client Client { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var client =  await _context.Clients.FirstOrDefaultAsync(c => c.Id == id && c.UserId == userId);
            
            if (client == null)
            {
                return NotFound();
            }
            Client = client;

            //ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return Page();
        }

        
        public async Task<IActionResult> OnPostAsync()
        {
            // Setare User
            Client.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);


            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Client).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ClientExists(Client.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool ClientExists(int id)
        {
            return _context.Clients.Any(e => e.Id == id);
        }
    }
}
