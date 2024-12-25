using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using radioCabs.Common;
using radioCabs.Constants;
using radioCabs.Data;
using radioCabs.Dtos;
using radioCabs.Dtos.Auth.Response;
using radioCabs.Dtos.User.Request;
using radioCabs.Models;
using radioCabs.Services.Auth;
using radioCabs.Services.User;

namespace radioCabs.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        private readonly ICommonService<User> _commonService;

        private readonly UserService _userService;

        private readonly IWebHostEnvironment _env;

        public UserController(ApplicationDbContext dbContext, ICommonService<User> commonService, IWebHostEnvironment env, UserService userService)
        {
            _context = dbContext;
            _commonService = commonService;
            _env = env;
            _userService = userService;
        }
        //method to search the user  with parameter class QueryParams
        [HttpPost("search")]
        public async Task<IActionResult> Get([FromBody] QueryParams queryParams)
        {
            List<string> roles = new List<string>() { UserRole.ADMIN };
            await _userService.CheckRole(roles);

            var query = _context.Users.AsQueryable();

            var pagedResponse = await _commonService.GetPagedDataAsync(queryParams, new[] { "Username", "Email" });

            return Ok(pagedResponse);
        }


        //method to find by id the user  with parameter id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            List<string> roles = new List<string>() { UserRole.ADMIN, UserRole.DRIVER, UserRole.COMPANY };
            await _userService.CheckRole(roles);

            var item = await _context.Users
                 .Where(u => u.Id.Equals(id))
                 .Select(u => new
                 {
                     u.Id,
                     u.Username,
                     u.Email,
                     u.Role,
                     u.Images,
                     u.CreatedAt,
                     u.LastLoginDate,
                 })
                 .FirstOrDefaultAsync();
            if (item == null)
            {
                return new JsonResult("User id Not found") { StatusCode = 400 };
            }

            return Ok(item);

        }
        //method to add new the user with parameter CreateUserDTO class
        [HttpPost]
        public async Task<IActionResult> Add([FromForm] CreateUserDTO acc)
        {

            List<string> roles = new List<string>() { UserRole.ADMIN };
            await _userService.CheckRole(roles);

            var checkExist = await _context.Users.FirstOrDefaultAsync(ct => ct.Email.Equals(acc.Email));
            if (checkExist != null)
            {
                return new JsonResult("Email is existed") { StatusCode = 400 };
            }
            //upload image
            var avatar = await _commonService.UploadFile(acc.Images);

            if (!acc.Equals(null))
            {
                var newUser = new User
                {
                    Email = acc.Email,
                    Username = acc.Username,
                    Role = acc.Role,
                    Images = avatar
                };
                if (acc.Password != null)
                {
                    newUser.Password = UtilityClass.MD5Hash(acc.Password);

                }

                try
                {
                   
                    _context.Users.Add(newUser);
                    await _context.SaveChangesAsync();


                    return CreatedAtAction("GetById", new { newUser.Id }, acc);;
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

            return new JsonResult("Something went wrong") { StatusCode = 500 };
        }

        //method to update the user with parameter id and UpdateUserDTO class
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] UpdateUserDTO acc)
        {
            List<string> roles = new List<string>() { UserRole.ADMIN };
            await _userService.CheckRole(roles);

            User? exist = await _context.Users.FirstOrDefaultAsync(t => t.Id == id);


            if (exist == null)
            {
                return BadRequest($"User with ID: {id} not found");
            }

            var avatar = await _commonService.UploadFile(acc.Images);

            if (ModelState.IsValid)
            {
                exist.Username = acc.Username;
                exist.Email = acc.Email;
                if (acc.Password != null)
                {
                    exist.Password = UtilityClass.MD5Hash(acc.Password);
                }
                exist.Role = acc.Role;

                if (avatar != null && avatar.Length > 0)
                {
                    exist.Images = avatar;
                }
 
                await _context.SaveChangesAsync();
                return Ok(exist);
            }

            return new JsonResult("Something went wrong") { StatusCode = 500 };
        }

        //method to delete the user with parameter id
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            List<string> roles = new List<string>() { UserRole.ADMIN };
            await _userService.CheckRole(roles);

            User? exist = await _context.Users.FirstOrDefaultAsync(t => t.Id == id);

            if (exist == null)
            {
                return BadRequest($"User with ID: {id} not found");
            }

            if (ModelState.IsValid)
            {
                _context.Remove(exist);
                await _context.SaveChangesAsync();

                return Ok(exist);
            }
            return new JsonResult("Something went wrong") { StatusCode = 500 };

        }
      /*  
        [AllowAnonymous]

        [Route("getimage/{fileName}")]
        public IActionResult GetImage(string fileName)
        {
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", fileName);

            if (!System.IO.File.Exists(filePath))
            {
                return Ok("thành công");
            }

            var image = System.IO.File.OpenRead(filePath);
            return File(image, "image/jpeg");
        }*/
    }
}
