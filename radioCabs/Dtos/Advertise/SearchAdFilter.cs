namespace radioCabs.Dtos.Advertise
{
    public class SearchAdFilter : QueryParams
    {
        public string? FromDate { get; set; }
        public string? ToDate { get; set; }
        public int CompanyId { get; set; }
    }
}
