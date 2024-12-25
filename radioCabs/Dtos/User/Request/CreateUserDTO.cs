using System.ComponentModel.DataAnnotations;

namespace radioCabs.Dtos.User.Request
{
    public class CreateUserDTO
    {
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
        [Required]
        public string? Username { get; set; }
        [Required]
        public string? Password { get; set; }
        [Required]
        public string? Role { get; set; }

        public IFormFile? Images { get; set; }
    }
}
