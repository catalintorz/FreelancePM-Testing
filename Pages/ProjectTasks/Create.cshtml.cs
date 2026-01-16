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

namespace FreelancePM.Pages.ProjectTasks
{
    public class CreateModel : PageModel
    {
        private readonly FreelancePM.Data.ApplicationDbContext _context;

        public CreateModel(FreelancePM.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public WorkTask WorkTask { get; set; } = default!;

        //public SelectList ProjectsList { get; set; } = default!;
        //public SelectList StatusesList { get; set; } = default!;


        public IActionResult OnGet()
        {
            PopulateDropdowns();

            //ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return Page();
        }
        
        public async Task<IActionResult> OnPostAsync()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            WorkTask.UserId = userId;

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
                PopulateDropdowns();
                return Page();
            }

            _context.WorkTasks.Add(WorkTask);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }

        private void PopulateDropdowns()
        {            

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);            

            ViewData["ProjectId"] = new SelectList(
                _context.Projects.Where(p => p.UserId == userId),
                "Id", "Name");

            ViewData["StatusId"] = new SelectList(
                _context.Statuses.ToList(),
                "Id", "Name");
        }
    }
}
