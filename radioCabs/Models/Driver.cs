using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace radioCabs.Models
{
    public class Driver
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; }

        public string? DriverName { get; set; }

        public string? ContactPerson { get; set; }

        public string? Address { get; set; }

        public string? City { get; set; }

        public string? Mobile { get; set; }

        public string? Telephone { get; set; }

        public string? Email { get; set; }

        public int Experience { get; set; }

        public string? Description { get; set; }

        public bool? IsActive { get; set; }

        public string? Images { get; set; }

        public DateTime? RegistrationDate { get; set; } = DateTime.Now;
    }
}
