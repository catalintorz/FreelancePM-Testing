using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace FreelancePM.Models
{
    public class WorkTask
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Titlul taskului este obligatoriu")]
        [StringLength(150)]
        public string Title { get; set; }

        [StringLength(1000)]
        public string Description { get; set; }

        [Required(ErrorMessage = "Deadline-ul este obligatoriu")]
        [DataType(DataType.Date)]        
        public DateTime Deadline { get; set; }

        //  Status
        [Required(ErrorMessage = "Statusul este obligatoriu")]
        public int StatusId { get; set; }
        public Status? Status { get; set; }

        // 🔗 Proiect
        [Required(ErrorMessage = "Proiectul este obligatoriu")]
        public int ProjectId { get; set; }
        [ValidateNever]
        public Project? Project { get; set; }

        // 🔐 Utilizator
        public string UserId { get; set; } = string.Empty;
        [ValidateNever]
        public IdentityUser? User { get; set; }

    }
}
