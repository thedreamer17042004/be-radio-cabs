namespace radioCabs.Dtos.PaymentPlan.Request
{
    public class UpdatePaymentPlanDTO
    {
        public string? PlanType { get; set; }

        public string? Duration { get; set; }

        public decimal? Amount { get; set; }

        public bool? IsActive { get; set; }
    }
}
