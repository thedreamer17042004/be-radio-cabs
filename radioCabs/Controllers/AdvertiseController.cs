using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using radioCabs.Common;
using radioCabs.Constants;
using radioCabs.Data;
using radioCabs.Dtos.Advertise;
using radioCabs.Dtos.Advertise.Request;
using radioCabs.Models;
using radioCabs.Services.User;

namespace radioCabs.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class AdvertiseController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        private readonly ICommonService<Advertise> _commonService;

        private readonly UserService _userService;

        public AdvertiseController(ApplicationDbContext dbContext, ICommonService<Advertise> commonService, UserService userService)
        {
            _context = dbContext;
            _commonService = commonService;
            _userService = userService;
        }
        //method to search the advertise with parameter queryParams
        [HttpPost("search")]
        public async Task<IActionResult> Get([FromBody] SearchAdFilter queryParams)
        {
            List<string> roles = new List<string>() { UserRole.ADMIN };
            await _userService.CheckRole(roles);

            var query = _context.Advertisements.AsQueryable();
            var includeFunc = new Func<IQueryable<Advertise>, IQueryable<Advertise>>(query =>
                 query.Include(p => p.Company)
            );

            var pagedResponse = await _commonService.SearchAd(queryParams, new[] { "CompanyId" }, includeFunc);

            return Ok(pagedResponse);
        }

        //method to find by id of the advertise with the parameter id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            List<string> roles = new List<string>() { UserRole.ADMIN };
            await _userService.CheckRole(roles);

            Advertise? item = await _context.Advertisements
                .Include(c => c.Company)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (item == null)
            {
                return new JsonResult("Advertise id Not found") { StatusCode = 400 };
            }

            return Ok(item);

        }
        //method to create the advertise with the parameter class CreateAdvertiseDTO
        [HttpPost]
        public async Task<IActionResult> Add([FromForm] CreateAdvertiseDTO adver)
        {
            List<string> roles = new List<string>() { UserRole.ADMIN, UserRole.COMPANY };
            await _userService.CheckRole(roles);

            var checkExist = await _context.Companies.FirstOrDefaultAsync(ct => ct.Id.Equals(adver.CompanyId));

            if (checkExist == null)
            {
                return new JsonResult("Company is not existed") { StatusCode = 400 };
            }

            var image = await _commonService.UploadFile(adver.Images);


            if (!adver.Equals(null))
            {
              
                Advertise item = new Advertise()
                {
                   CompanyId = adver.CompanyId,
                   Description = adver.Description,
                   Designation = adver.Designation,
                   IsActive = adver.IsActive,
                   StartDate = adver.StartDate,
                   EndDate = adver.EndDate,
                   Images = image
                };

        

                await _context.Advertisements.AddAsync(item);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetById", new { item.Id }, adver);
            }

            return new JsonResult("Something went wrong") { StatusCode = 500 };
        }
        //method to update advertise with the parameter class UpdateAdvertiseDTO and id
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] UpdateAdvertiseDTO adver)
        {
            List<string> roles = new List<string>() { UserRole.ADMIN };
            await _userService.CheckRole(roles);

            Advertise? exist = await _context.Advertisements.FirstOrDefaultAsync(t => t.Id == id);


            if (exist == null)
            {
                throw new ArgumentException($"Advertisement with ID: {id} not found");
            }

            var image = await _commonService.UploadFile(adver.Images);


            if (ModelState.IsValid)
            {
                var checkExist = await _context.Companies.FirstOrDefaultAsync(ct => ct.Id.Equals(adver.CompanyId));

                if (checkExist == null)
                {
                    return new JsonResult("Company is not existed") { StatusCode = 400 };
                }

                exist.CompanyId = adver.CompanyId;
                exist.Designation = adver.Designation;
                exist.Description = adver.Description;
                exist.StartDate = adver.StartDate;
                exist.EndDate = adver.EndDate;
                exist.IsActive = adver.IsActive;

                if (image != null && image.Length > 0)
                {
                    exist.Images = image;
                }

                await _context.SaveChangesAsync();
                return Ok(exist);
            }

            return new JsonResult("Something went wrong") { StatusCode = 500 };
        }
        //method to delete advertise with the parameter id
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            List<string> roles = new List<string>() { UserRole.ADMIN };
            await _userService.CheckRole(roles);

            Advertise? exist = await _context.Advertisements.FirstOrDefaultAsync(t => t.Id == id);

            if (exist == null) return new JsonResult($"Advertisement with ID: {id} not found");

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
