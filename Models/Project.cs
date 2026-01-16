using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FreelancePM.Models
{
    public class Project
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Numele proiectului este obligatoriu")]
        [StringLength(150)]
        public string Name { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Data start")]
        public DateTime StartDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Deadline-ul este obligatoriu")]
        [DataType(DataType.Date)]
        [Display(Name = "Deadline")]
        public DateTime Deadline { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Bugetul trebuie să fie pozitiv")]
        public decimal Budget { get; set; }

        //  Client
        [Required]
        public int ClientId { get; set; }
        [ValidateNever]
        public Client? Client { get; set; }

        // Utilizator        
        public string? UserId { get; set; }
        [ValidateNever]
        public IdentityUser? User { get; set; }

        // Taskuri       
        [ValidateNever]
        public ICollection<WorkTask> WorkTasks { get; set; }
    }
}
