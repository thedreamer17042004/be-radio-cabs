using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using radioCabs.Configs;
using radioCabs.Data;
using radioCabs.Dtos.Auth;
using radioCabs.Services.User;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace radioCabs.Services.Auth
{
    public class JwtService : IJwtService
    {
        private readonly JwtConfig _jwtConfig;
        private readonly ApplicationDbContext _context;
        private readonly TokenValidationParameters _tokenValidationParameters;
        private readonly UserService _userService;
        public JwtService(
            IOptionsMonitor<JwtConfig> jwtConfig, 
            ApplicationDbContext context,
            UserService userService, 
            TokenValidationParameters tokenValidationParameters
            )
        {
            _jwtConfig = jwtConfig.CurrentValue;
            _context = context;
            _tokenValidationParameters = tokenValidationParameters;
            _userService = userService;

        }
        //the method to generate token to authentication
        public async Task<AuthResult> GenerateToken(radioCabs.Models.User user)
        {
            JwtSecurityTokenHandler? jwtTokenHandler = new JwtSecurityTokenHandler();
            string userRole = await _userService.GetUserRole(user.Id);
            int userId = user.Id;
            byte[] key = Encoding.ASCII.GetBytes(_jwtConfig.Secret);

            var claims = new List<Claim>
            {
                    new Claim("Id", userId+""),
                    new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.Role, userRole)
            };

            var claimsIdentity = new ClaimsIdentity(claims);

            SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = claimsIdentity,
                Expires = DateTime.UtcNow.AddHours(24),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };
          
            SecurityToken? token = jwtTokenHandler.CreateToken(tokenDescriptor);
            string jwtToken = jwtTokenHandler.WriteToken(token);

         
            return new AuthResult()
            {
                Token = jwtToken,
                Success = true
            };
        }
    }
}
