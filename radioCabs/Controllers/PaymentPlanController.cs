using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using radioCabs.Common;
using radioCabs.Constants;
using radioCabs.Data;
using radioCabs.Dtos.PaymentPlan;
using radioCabs.Dtos.PaymentPlan.Request;
using radioCabs.Models;
using radioCabs.Services.User;

namespace radioCabs.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentPlanController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        private readonly ICommonService<PaymentPlan> _commonService;

        private readonly UserService _userService;

        public PaymentPlanController(ApplicationDbContext dbContext, ICommonService<PaymentPlan> commonService, UserService userService)
        {
            _context = dbContext;
            _commonService = commonService;
            _userService = userService;
        }

        //method to search the payment plan with parameter class SearchPaymentPlanFilter
        [HttpPost("search")]
        public async Task<IActionResult> Get([FromBody] SearchPaymentPlanFilter queryParams)
        {
            List<string> roles = new List<string>() { UserRole.ADMIN };
            await _userService.CheckRole(roles);

            var query = _context.PaymentPlans.AsQueryable();

            var pagedResponse = await _commonService.SearchPaymentPlan(queryParams, new[] { "" });

            return Ok(pagedResponse);
        }

        //method to find the payment plan with parameter id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            List<string> roles = new List<string>() { UserRole.ADMIN };
            await _userService.CheckRole(roles);

            PaymentPlan? item = await _context.PaymentPlans
                .FirstOrDefaultAsync(c => c.Id == id);

            if (item == null)
            {
                return new JsonResult("PaymentPlan id Not found") { StatusCode = 400 };
            }

            return Ok(item);

        }
        //method to add new the payment plan with parameter CreatePaymentPlanDTO class
        [HttpPost]
        public async Task<IActionResult> Add([FromForm] CreatePaymentPlanDTO paymentPlan)
        {
            List<string> roles = new List<string>() { UserRole.ADMIN };
            await _userService.CheckRole(roles);


            if (!paymentPlan.Equals(null))
            {

                PaymentPlan item = new PaymentPlan()
                {
                    Amount = paymentPlan.Amount,
                    Duration = paymentPlan.Duration,
                    PlanType = paymentPlan.PlanType,
                   IsActive = paymentPlan.IsActive 
                };

                await _context.PaymentPlans.AddAsync(item);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetById", new { item.Id }, paymentPlan);
            }

            return new JsonResult("Something went wrong") { StatusCode = 500 };
        }

        //method to update the payment plan with parameter UpdatePaymentPlanDTO class and id
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] UpdatePaymentPlanDTO paymentPlan)
        {
            List<string> roles = new List<string>() { UserRole.ADMIN };
            await _userService.CheckRole(roles);

            PaymentPlan? exist = await _context.PaymentPlans.FirstOrDefaultAsync(t => t.Id == id);


            if (exist == null)
            {
                throw new ArgumentException($"Payment plan with ID: {id} not found");
            }


            if (ModelState.IsValid)
            {
                exist.Amount = paymentPlan.Amount;
                exist.Duration = paymentPlan.Duration;
                exist.PlanType = paymentPlan.PlanType;
                exist.IsActive = paymentPlan.IsActive;

                await _context.SaveChangesAsync();
                return Ok(exist);
            }

            return new JsonResult("Something went wrong") { StatusCode = 500 };
        }
        //method to delete the payment plan with parameter id
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            List<string> roles = new List<string>() { UserRole.ADMIN };
            await _userService.CheckRole(roles);

            PaymentPlan? exist = await _context.PaymentPlans.FirstOrDefaultAsync(t => t.Id == id);

            if (exist == null) return new JsonResult($"Payment plan with ID: {id} not found");

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
