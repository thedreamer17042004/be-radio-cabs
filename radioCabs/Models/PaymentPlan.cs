using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace radioCabs.Models
{
    public class PaymentPlan
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string? PlanType { get; set; }

        public string? Duration { get; set; }

        public decimal? Amount { get; set; }

        public bool? IsActive { get; set; }

    }
}
