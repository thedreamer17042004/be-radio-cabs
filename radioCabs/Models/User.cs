using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace radioCabs.Models
{
    
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string? Username { get; set; }

        public string? Password { get; set; }

        public string? Email { get; set; }

        public string? Role { get; set; }

        public string? Images { get; set; }

        public DateTime? CreatedAt { get; set; } = DateTime.Now;

        public DateTime? LastLoginDate { get; set; }
    }
}
