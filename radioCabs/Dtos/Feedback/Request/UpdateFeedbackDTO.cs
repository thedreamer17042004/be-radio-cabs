namespace radioCabs.Dtos.Feedback.Request
{
    public class UpdateFeedbackDTO
    {
        public string? Name { get; set; }

        public string? Mobile { get; set; }

        public string? Email { get; set; }

        public string? City { get; set; }

        public string? FeedbackType { get; set; }

        public string? Description { get; set; }

        public IFormFile? Images { get; set; }
    }
}
