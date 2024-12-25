using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using radioCabs.Data;
using radioCabs.Dtos.Auth;
using radioCabs.Dtos.Auth.Request;
using radioCabs.Dtos.Auth.Response;
using radioCabs.Models;
using radioCabs.Services.Auth;
using radioCabs.Services.User;

namespace radioCabs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
       private readonly ApplicationDbContext _context;
       private readonly IJwtService _jwtService;

        public AuthController(IJwtService jwtService, ApplicationDbContext context)
        {
            _context = context;
            _jwtService = jwtService;
        }

        //method to register user with parameter class  RegisterUserDTO
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterUserDTO user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new RegisterResponseDTO
                {
                    Errors = new List<string> { "Invalid payload" },
                    Success = false
                });
            }

            // Check if the user already exists
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email || u.Username == user.Username);

            if (existingUser != null)
            {
                return BadRequest(new RegisterResponseDTO
                {
                    Errors = new List<string> { "Email or username already registered" },
                    Success = false
                });
            }

            var newUser = new User
            {
                Email = user.Email,
                Username = user.Username,
                Role = user.Role
            };
            if (user.Password != null)
            {
                newUser.Password = UtilityClass.MD5Hash(user.Password);
            }

            try
            {
                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                var authResult = await _jwtService.GenerateToken(newUser);

                return Ok(authResult);
            }
            catch (Exception ex)
            {
               
                Console.Error.WriteLine(ex.Message);

                return StatusCode(500, new RegisterResponseDTO
                {
                    Errors = new List<string> { "An error occurred while processing your request" },
                    Success = false
                });
            }
        }
        //method to login user with parameter class LoginUserDTO
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginUserDTO user)
        {
            if (ModelState.IsValid)
            {
                User? existingUser = await _context.Users.FirstOrDefaultAsync(item=> item.Email == user.Email);

                if (existingUser == null)
                {
                    return BadRequest(new RegisterResponseDTO()
                    {
                        Errors = new List<string>() { "Email address is not registered." },
                        Success = false
                    });
                }
                var hashPassword = "";
                if (user.Password != null) {
                    hashPassword = UtilityClass.MD5Hash(user.Password);
                }
                var acc = _context.Users.FirstOrDefault(x => x.Email == user.Email && x.Password == hashPassword);


                if (acc!=null)
                {
                    acc.LastLoginDate = DateTime.Now;
                    _context.SaveChanges();

                    AuthResult authResult = await _jwtService.GenerateToken(existingUser);

                    return Ok(authResult);
                }
                else
                {
                    return BadRequest(new RegisterResponseDTO()
                    {
                        Errors = new List<string>() { "Wrong password" },
                        Success = false
                    });
                }
            }

            return BadRequest(new RegisterResponseDTO()
            {
                Errors = new List<string>() { "Invalid payload" },
                Success = false
            });
        }
    }
}
