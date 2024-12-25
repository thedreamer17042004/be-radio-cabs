using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using radioCabs.Common;
using radioCabs.Constants;
using radioCabs.Data;
using radioCabs.Dtos;
using radioCabs.Dtos.Company.Request;
using radioCabs.Models;
using radioCabs.Services.User;

namespace radioCabs.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class CompanyController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        private readonly ICommonService<Company> _commonService;

        private readonly UserService _userService;

        public CompanyController(ApplicationDbContext dbContext, ICommonService<Company> commonService, UserService userService)
        {
            _context = dbContext;
            _commonService = commonService;
            _userService = userService;
        }
        //method to search the company with parameter class QueryParams
        [AllowAnonymous]
        [HttpPost("search")]
        public async Task<IActionResult> Get([FromBody] QueryParams queryParams)
        {
            var query = _context.Companies.AsQueryable();

            var pagedResponse = await _commonService.GetPagedDataAsync(queryParams, new[] { "CompanyName", "Mobile", "Telephone", "Email", "MemberShipType" });

            return Ok(pagedResponse);
        }

        //method to find by id the company with parameter id
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
        
            Company? item = await _context.Companies
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (item == null)
            {
                return new JsonResult("Company id Not found") { StatusCode = 400 };
            }

            return Ok(item);

        }
        //method to create the company with parameter class CreateCompanyDTO
        [HttpPost]
        public async Task<IActionResult> Add([FromForm] CreateCompanyDTO company)
        {
            List<string> roles = new List<string>() { UserRole.ADMIN, UserRole.COMPANY };
            await _userService.CheckRole(roles);

            var checkExist = await _context.Companies.FirstOrDefaultAsync(ct => ct.CompanyName.Equals(company.CompanyName) || ct.Email.Equals(company.Email));

            if (checkExist != null)
            {
                return new JsonResult("Company is existed") { StatusCode = 400 };
            }

            
            var imageCompany = await _commonService.UploadFile(company.Images);
   

            if (!company.Equals(null))
            {
             
                var existUserId =  await _context.Users.FirstOrDefaultAsync(item => item.Id == company.UserId);
                if (existUserId == null) {
                    throw new Exception("Lỗi server");
                }

                Company item = new Company()
                {
                 Address= company.Address,
                 CompanyName=company.CompanyName,
                 ContactPerson=company.ContactPerson,
                 Email = company.Email,
                 Designation = company.Designation,
                 FaxNumber  =company.FaxNumber,
                 IsActive=company.IsActive,
                 MembershipType = company.MembershipType,
                 Mobile = company.Mobile,
                 Telephone = company.Telephone,
                 UserId= company.UserId,
                 Images = imageCompany,
                 RegistrationDate = DateTime.Now
                };

                await _context.Companies.AddAsync(item);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetById", new { item.Id }, company);
            }

            return new JsonResult("Something went wrong") { StatusCode = 500 };
        }
        //method to update the company with parameter class UpdateDriverDTO
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] UpdateCompanyDTO company)
        {
            List<string> roles = new List<string>() { UserRole.ADMIN, UserRole.COMPANY };
            await _userService.CheckRole(roles);

            Company? exist =  await _context.Companies.FirstOrDefaultAsync(t => t.Id == id);


            if (exist == null)
            {
                throw new ArgumentException($"Compnay with ID: {id} not found");
            }
          
            var imageCompany = await _commonService.UploadFile(company.Images);

    
            if (ModelState.IsValid)
            {
                var existUserId = await _context.Users.FirstOrDefaultAsync(item => item.Id == company.UserId);
                if (existUserId == null)
                {
                    throw new Exception("Lỗi server");
                }
                exist.Telephone = company.Telephone;
                exist.Mobile = company.Mobile;
                exist.UserId = company.UserId;
                exist.Address = company.Address;
                exist.FaxNumber = company.FaxNumber;
                exist.MembershipType = company.MembershipType;
                exist.CompanyName = company.CompanyName;
                exist.Address = company.Address;
                exist.ContactPerson = company.ContactPerson;
                exist.Email = company.Email;
                exist.IsActive = company.IsActive;
                exist.UserId = company.UserId;


                if (imageCompany != null && imageCompany.Length > 0)
                {
                    exist.Images = imageCompany;
                }
               
                await _context.SaveChangesAsync();
                return Ok(exist);
            }

            return new JsonResult("Something went wrong") { StatusCode = 500 };
        }
        //method to delete the company with parameter id
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            List<string> roles = new List<string>() { UserRole.ADMIN };
            await _userService.CheckRole(roles);

            Company? exist = await _context.Companies.FirstOrDefaultAsync(t => t.Id == id);

            if (exist==null) return new JsonResult($"Product with ID: {id} not found");

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
