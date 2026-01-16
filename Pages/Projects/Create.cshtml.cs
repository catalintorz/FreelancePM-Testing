using FreelancePM.Data;
using FreelancePM.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace FreelancePM.Pages.Projects
{
    public class CreateModel : PageModel
    {
        private readonly FreelancePM.Data.ApplicationDbContext _context;

        public CreateModel(FreelancePM.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewData["ClientId"] = new SelectList( _context.Clients.Where(c => c.UserId == userId), "Id", "Name");
            //ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return Page();
        }

        [BindProperty]
        public Project Project { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            Project.UserId = userId;

            if (!ModelState.IsValid)
            {
                Debug.WriteLine("INVALID", "Error");
                foreach (var entry in ModelState)
                {
                    foreach (var error in entry.Value.Errors)
                    {
                        Debug.WriteLine($"FIELD: {entry.Key} - ERROR: {error.ErrorMessage}", "Error");
                    }
                }
                return Page();
            }

            _context.Projects.Add(Project);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
