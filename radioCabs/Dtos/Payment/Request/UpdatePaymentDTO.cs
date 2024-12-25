namespace radioCabs.Dtos.Payment.Request
{
    public class UpdatePaymentDTO
    {
        public int UserId { get; set; }

        public int PlanId { get; set; }

        public decimal? Amount { get; set; }

        public DateTime? PaymentDate { get; set; }

        public string? PaymentStatus { get; set; }

        public string? PaymentType { get; set; }

        public DateTime? ValidFrom { get; set; }

        public DateTime? ValidTo { get; set; }
    }
}
