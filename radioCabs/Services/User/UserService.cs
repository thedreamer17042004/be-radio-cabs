using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using radioCabs.Configs;
using radioCabs.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace radioCabs.Services.User
{
    public class UserService
    {
        private readonly ApplicationDbContext _context;
        private readonly string? _secretKey;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UserService(ApplicationDbContext context, IOptions<JwtConfig> jwtConfig, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _secretKey = jwtConfig.Value.Secret;
        }

        /*
         the method to get role of the user
         */
        public async Task<string> GetUserRole(int userId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(item => item.Id == userId);
            if (user == null)
            {
                throw new ArgumentException("User not found");
            }

            return user.Role;
        }

        /*
         the method to get current role of the user when logon
         */
        private string GetCurrentRole(string token)
        {

            try
            {
                var principal = ValidToken(token, _secretKey);
                var roleClaim = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role || c.Type == "role");

                if (roleClaim == null)
                {
                    throw new Exception("Role claim not found in the token");
                }

                return roleClaim.Value;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error decoding token: {ex.Message}");
                return null;
            }
        }
        /*the method to check role of the user*/
        public async Task CheckRole(List<string> roles)
        {
            var token = GetTokenFromHeader();
            var currentRole = GetCurrentRole(token);
            var roleCheckResult = await CheckRole(currentRole, roles);
            if (roleCheckResult != null)
            {
                throw new Exception("Forbidden");
            }
            await Task.CompletedTask;
        }

        private async Task<string> CheckRole(string role, List<string> roleName)
        {

            if (!roleName.Contains(role))
            {
      
                return "forbidden";

            }

            return null;
        }
        /*
         to valid token from authorization header
         */
        private ClaimsPrincipal ValidToken(string token, string? secretKey)
        {
            try
            {

                var handler = new JwtSecurityTokenHandler();

            
                if (!handler.CanReadToken(token))
                {
                    throw new ArgumentException("Invalid JWT token");
                }

         
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false, 
                    ValidateAudience = false, 
                    ValidateLifetime = true, 
                    ValidateIssuerSigningKey = true, 
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
                };

                SecurityToken validatedToken;
                var principal = handler.ValidateToken(token, validationParameters, out validatedToken);

                return principal;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        //the method to get token from header
        private string GetTokenFromHeader()
        {
            if (_httpContextAccessor.HttpContext != null &&
                _httpContextAccessor.HttpContext.Request.Headers.TryGetValue("Authorization", out var authorizationHeader))
            {
                var token = authorizationHeader.ToString().StartsWith("Bearer ")
                    ? authorizationHeader.ToString().Substring("Bearer ".Length).Trim()
                    : authorizationHeader.ToString();

                return token;
            }

            throw new Exception("Authorization header is missing.");
        }

    }
}
