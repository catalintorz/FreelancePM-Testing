using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreelancePM.Models
{
    public class Client
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Client name is required")]
        [StringLength(100)]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email address is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Invalid phone number")]
        public string? Phone { get; set; }

        [StringLength(100)]
        public string? Company { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        // Legtura cu utilizatorul  
        public string? UserId { get; set; }
        
        [ValidateNever]
        public IdentityUser? User { get; set; }

        [ValidateNever]
        public ICollection<Project> Projects { get; set; } = new List<Project>();

    }
}
