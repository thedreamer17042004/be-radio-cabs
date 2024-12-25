using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace radioCabs.Models
{
    public class Feedback
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? Mobile { get; set; }

        public string? Email { get; set; }

        public string? City { get; set; }

        public string? FeedbackType { get; set; }

        public string? Description { get; set; }

        public string? Images { get;set; }

        public DateTime? SubmissionDate { get; set; } = DateTime.Now;
    }
}
