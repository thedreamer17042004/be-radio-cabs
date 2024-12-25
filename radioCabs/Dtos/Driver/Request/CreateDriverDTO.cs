
namespace radioCabs.Dtos.Driver.Request
{
    public class CreateDriverDTO
    {
        public int UserId { get; set; }

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

        public IFormFile? Images { get; set; }

        public DateTime? RegistrationDate { get; set; } = DateTime.Now;
    }
}
