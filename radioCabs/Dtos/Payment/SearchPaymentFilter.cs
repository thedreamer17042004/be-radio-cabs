namespace radioCabs.Dtos.Payment
{
    public class SearchPaymentFilter:QueryParams
    {
        public string? PaymentDate { get; set; }
        public string? PaymentStatus { get; set; }
        public string? PaymentType { get; set; }
        public string? PlanId { get; set; }
    }
}
