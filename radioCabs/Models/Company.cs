using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace radioCabs.Models
{
    public class Company
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; }

        public string? CompanyName { get; set; }

        public string? ContactPerson { get; set; }

        public string? Designation { get; set; }

        public string? Address { get; set; }

        public string? Mobile { get; set; }

        public string? Telephone { get; set; }

        public string? FaxNumber { get; set; }

        public string? Email { get; set; }

        public string? MembershipType { get; set; }

        public bool? IsActive { get; set; }

        public string? Images { get; set; }

        public DateTime? RegistrationDate { get; set; } = DateTime.Now;
    }
}
