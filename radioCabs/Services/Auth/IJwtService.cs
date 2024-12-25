using radioCabs.Dtos.Auth;

namespace radioCabs.Services.Auth
{
    public interface IJwtService
    {
        Task<AuthResult> GenerateToken(radioCabs.Models.User user);
    }
}
