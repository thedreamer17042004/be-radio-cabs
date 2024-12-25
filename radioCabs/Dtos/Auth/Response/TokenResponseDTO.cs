namespace radioCabs.Dtos.Auth.Response
{
    public class TokenResponseDTO
    {
        public string? UserId { get; set; }
        public bool Success { get; set; }
        public List<string>? Errors { get; set; }
    }
}
