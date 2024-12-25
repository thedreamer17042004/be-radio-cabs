using System.ComponentModel.DataAnnotations;

namespace radioCabs.Dtos.User.Request
{
    public class UpdateUserDTO
    {
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
        [Required]
        public string? Username { get; set; }
        public string? Password { get; set; }
        [Required]
        public string? Role { get; set; }

        public IFormFile? Images { get; set; }
    }
}
