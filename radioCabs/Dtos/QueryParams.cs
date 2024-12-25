namespace radioCabs.Dtos
{
    public class QueryParams
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? Keyword { get; set; }
        public string? Status { get; set; }
        public string SortBy { get; set; } = "Id";
        public string SortDir { get; set; } = "asc";
    }
}
