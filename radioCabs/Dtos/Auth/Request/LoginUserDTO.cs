using System.ComponentModel.DataAnnotations;

namespace radioCabs.Dtos.Auth.Request
{
    public class LoginUserDTO
    {
        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        public string? Password { get; set; }
    }
}
