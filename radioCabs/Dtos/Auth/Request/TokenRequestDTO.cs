using System.ComponentModel.DataAnnotations;

namespace radioCabs.Dtos.Auth.Request
{
    public class TokenRequestDTO
    {
        [Required]
        public string? Token { get; set; }
    }
}
