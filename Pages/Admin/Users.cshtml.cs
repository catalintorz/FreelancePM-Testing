using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace FreelancePM.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class UsersModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;

        public UsersModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public List<IdentityUser> AllUsers { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public bool Export { get; set; } = false;

        public async Task<IActionResult> OnGetAsync()
        {
            AllUsers = await _userManager.Users
                .OrderBy(u => u.UserName)
                .ToListAsync();

            // EXPORT CSV
            if (Export)
            {
                var sb = new StringBuilder();
                sb.AppendLine("Email,Username");

                foreach (var u in AllUsers)
                {
                    sb.AppendLine($"{u.Email},{u.UserName}");
                }

                var bytes = Encoding.UTF8.GetBytes(sb.ToString());
                return File(bytes, "text/csv", "users.csv");
            }

            return Page();
        }
    }
}
