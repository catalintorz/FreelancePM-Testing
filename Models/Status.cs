using System.ComponentModel.DataAnnotations;

namespace FreelancePM.Models
{
    public class Status
    {

        public int Id { get; set; }

        [Required(ErrorMessage = "Status name is required")]
        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(200)]
        public string? Description { get; set; }

    }
}
