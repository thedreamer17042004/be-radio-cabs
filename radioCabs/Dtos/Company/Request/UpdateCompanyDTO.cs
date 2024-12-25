using System.ComponentModel.DataAnnotations;

namespace radioCabs.Dtos.Company.Request
{
    public class UpdateCompanyDTO
    {
        [Required]
        public string? CompanyName { get; set; }

        public int UserId { get; set; }

        public string? ContactPerson { get; set; }

        public string? Designation { get; set; }

        public string? Address { get; set; }

        public string? Mobile { get; set; }

        public string? Telephone { get; set; }

        public string? FaxNumber { get; set; }

        public string? Email { get; set; }

        public string? MembershipType { get; set; }

        public bool IsActive { get; set; }


        public IFormFile? Images { get; set; }
    }
}
