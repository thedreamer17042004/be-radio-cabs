namespace radioCabs.Dtos.PaymentPlan
{
    public class SearchPaymentPlanFilter:QueryParams
    {
        public string? PlanType { get; set; }
        public string? Duration { get; set; }
    }
}
