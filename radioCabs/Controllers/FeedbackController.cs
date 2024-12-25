using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using radioCabs.Common;
using radioCabs.Constants;
using radioCabs.Data;
using radioCabs.Dtos;
using radioCabs.Dtos.Feedback.Request;
using radioCabs.Models;
using radioCabs.Services.User;

namespace radioCabs.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class FeedbackController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        private readonly ICommonService<Feedback> _commonService;

        private readonly UserService _userService;

        public FeedbackController(ApplicationDbContext dbContext, ICommonService<Feedback> commonService, UserService userService)
        {
            _context = dbContext;
            _commonService = commonService;
            _userService = userService;
        }
        //method to search the feedback with parameter class QueryParams
        [HttpPost("search")]
        public async Task<IActionResult> Get([FromBody] QueryParams queryParams)
        {
            List<string> roles = new List<string>() { UserRole.ADMIN };
            await _userService.CheckRole(roles);

            var query = _context.Feedback.AsQueryable();

            var pagedResponse = await _commonService.GetPagedDataAsync(queryParams, new[] { "Name", "Mobile", "Email" });

            return Ok(pagedResponse);
        }

        //method to find by id the feedback with parameter id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            List<string> roles = new List<string>() { UserRole.ADMIN };
            await _userService.CheckRole(roles);

            Feedback? item = await _context.Feedback
                .FirstOrDefaultAsync(c => c.Id == id);

            if (item == null)
            {
                return new JsonResult("Feed back id Not found") { StatusCode = 400 };
            }

            return Ok(item);

        }
        //method to create the feedback with parameter CreateFeedbackDTO class
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Add([FromForm] CreateFeedbackDTO feedback)
        {
            var image = await _commonService.UploadFile(feedback.Images);


            if (!feedback.Equals(null))
            {

                Feedback item = new Feedback()
                {
                   Name = feedback.Name,
                   Email = feedback.Email,
                   City = feedback.City,
                   Description = feedback.Description,
                   FeedbackType = feedback.FeedbackType,
                   Images = image,
                   Mobile = feedback.Mobile,
                   SubmissionDate = DateTime.Now
                };

                await _context.Feedback.AddAsync(item);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetById", new { item.Id }, feedback);
            }

            return new JsonResult("Something went wrong") { StatusCode = 500 };
        }
        //method to update the feedback with parameter UpdateFeedbackDTO class
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] UpdateFeedbackDTO feedback)
        {
            List<string> roles = new List<string>() { UserRole.ADMIN };
            await _userService.CheckRole(roles);

            Feedback? exist = await _context.Feedback.FirstOrDefaultAsync(t => t.Id == id);


            if (exist == null)
            {
                throw new ArgumentException($"Feed back with ID: {id} not found");
            }

            var image = await _commonService.UploadFile(feedback.Images);


            if (ModelState.IsValid)
            {
                exist.Name = feedback.Name;
                exist.Email = feedback.Email;
                exist.City = feedback.City;
                exist.Description = feedback.Description;
                exist.FeedbackType = feedback.FeedbackType;
                exist.Images = image;
                exist.Mobile = feedback.Mobile;
                exist.SubmissionDate = DateTime.Now;

                if (image != null && image.Length > 0)
                {
                    exist.Images = image;
                }

                await _context.SaveChangesAsync();
                return Ok(exist);
            }

            return new JsonResult("Something went wrong") { StatusCode = 500 };
        }
        //method to delete the feedback with parameter id
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            List<string> roles = new List<string>() { UserRole.ADMIN };
            await _userService.CheckRole(roles);

            Feedback? exist = await _context.Feedback.FirstOrDefaultAsync(t => t.Id == id);

            if (exist == null) return new JsonResult($"Feed back with ID: {id} not found");

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
