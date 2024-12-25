using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using radioCabs.Common;
using radioCabs.Constants;
using radioCabs.Data;
using radioCabs.Dtos;
using radioCabs.Dtos.Driver.Request;
using radioCabs.Models;
using radioCabs.Services.User;

namespace radioCabs.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class DriverController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        private readonly ICommonService<Driver> _commonService;

        private readonly UserService _userService;

        public DriverController(ApplicationDbContext dbContext, ICommonService<Driver> commonService, UserService userService)
        {
            _context = dbContext;
            _commonService = commonService;
            _userService = userService;
        }
        //method to search the driver with parameter class QueryParams
        [AllowAnonymous]
        [HttpPost("search")]
        public async Task<IActionResult> Get([FromBody] QueryParams queryParams)
        {

            var query = _context.Drivers.AsQueryable();

            var pagedResponse = await _commonService.GetPagedDataAsync(queryParams, new[] { "DriverName", "Mobile", "Telephone", "Email" });

            return Ok(pagedResponse);
        }
        //method to find by id the driver with parameter id
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            Driver? item = await _context.Drivers
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (item == null)
            {
                return new JsonResult("Driver id Not found") { StatusCode = 400 };
            }

            return Ok(item);

        }

        //method to add new the driver with parameter class CreateDriverDTO
        [HttpPost]
        public async Task<IActionResult> Add([FromForm] CreateDriverDTO driver)
        {
            List<string> roles = new List<string>() { UserRole.ADMIN , UserRole.DRIVER};
            await _userService.CheckRole(roles);

            var checkExist = await _context.Drivers.FirstOrDefaultAsync(ct => ct.Email.Equals(driver.Email));

            if (checkExist != null)
            {
                return new JsonResult("Driver is existed") { StatusCode = 400 };
            }

            var image = await _commonService.UploadFile(driver.Images);


            if (!driver.Equals(null))
            {
                var existUserId = await _context.Users.FirstOrDefaultAsync(item => item.Id == driver.UserId);
                if (existUserId == null)
                {
                    throw new Exception("Lỗi server");
                }

                Driver item = new Driver()
                {
                  Address = driver.Address,
                  City = driver.City,
                  ContactPerson = driver.ContactPerson,
                  Description = driver.Description,
                  DriverName = driver.DriverName,
                  Email = driver.Email,
                  Images = image,
                  Experience = driver.Experience, 
                  IsActive = driver.IsActive,
                  Mobile = driver.Mobile,
                  Telephone = driver.Telephone,
                  UserId = driver.UserId,
                  RegistrationDate = DateTime.Now
                };

                await _context.Drivers.AddAsync(item);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetById", new { item.Id }, driver);
            }

            return new JsonResult("Something went wrong") { StatusCode = 500 };
        }
        //method to update the driver with parameter class UpdateDriverDTO
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] UpdateDriverDTO driver)
        {
            List<string> roles = new List<string>() { UserRole.ADMIN , UserRole.DRIVER };
            await _userService.CheckRole(roles);

            Driver? exist = await _context.Drivers.FirstOrDefaultAsync(t => t.Id == id);


            if (exist == null)
            {
                throw new ArgumentException($"Driver with ID: {id} not found");
            }

            var image = await _commonService.UploadFile(driver.Images);


            if (ModelState.IsValid)
            {
                var existUserId = await _context.Users.FirstOrDefaultAsync(item => item.Id == driver.UserId);
                if (existUserId == null)
                {
                    throw new Exception("Lỗi server");
                }
                exist.Telephone = driver.Telephone;
                exist.Address = driver.Mobile;
                exist.City = driver.City;
                exist.ContactPerson = driver.ContactPerson;
                exist.Description = driver.Description;
                exist.DriverName = driver.DriverName;
                exist.Email = driver.Email;
                exist.Experience = driver.Experience;
                exist.IsActive = driver.IsActive;
                exist.Mobile = driver.Mobile;
                exist.UserId = driver.UserId;

                if (image != null && image.Length > 0)
                {
                    exist.Images = image;
                }

                await _context.SaveChangesAsync();
                return Ok(exist);
            }

            return new JsonResult("Something went wrong") { StatusCode = 500 };
        }
        //method to delete the driver with parameter id
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            List<string> roles = new List<string>() { UserRole.ADMIN };
            await _userService.CheckRole(roles);

            Driver? exist = await _context.Drivers.FirstOrDefaultAsync(t => t.Id == id);

            if (exist == null) return new JsonResult($"Driver with ID: {id} not found");

            if (ModelState.IsValid)
            {
                _context.Remove(exist);
                await _context.SaveChangesAsync();

                return Ok(exist);
            }
            return new JsonResult("Something went wrong") { StatusCode = 500 };

        }
    }
}
