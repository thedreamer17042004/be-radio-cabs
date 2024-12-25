using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using radioCabs.Common;
using radioCabs.Constants;
using radioCabs.Data;
using radioCabs.Dtos.Payment;
using radioCabs.Dtos.Payment.Request;
using radioCabs.Models;
using radioCabs.Services.User;

namespace radioCabs.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        private readonly ICommonService<Payment> _commonService;

        private readonly UserService _userService;

        public PaymentController(ApplicationDbContext dbContext, ICommonService<Payment> commonService, UserService userService)
        {
            _context = dbContext;
            _commonService = commonService;
            _userService = userService;
        }
        //method to search the payment with parameter class SearchPaymentFilter
        [HttpPost("search")]
        public async Task<IActionResult> Get([FromBody] SearchPaymentFilter queryParams)
        {
            List<string> roles = new List<string>() { UserRole.ADMIN };
            await _userService.CheckRole(roles);

            var query = _context.PaymentPlans.AsQueryable();
            var includeFunc = new Func<IQueryable<Payment>, IQueryable<Payment>>(query =>
                query.Include(p => p.User).Include(p1=>p1.PaymentPlan)
            );
            var pagedResponse = await _commonService.SearchPayment(queryParams, new[] { "" }, includeFunc);

            return Ok(pagedResponse);
        }

        //method to find by id the payment with parameter id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            List<string> roles = new List<string>() { UserRole.ADMIN, UserRole.DRIVER, UserRole.COMPANY };
            await _userService.CheckRole(roles);

            Payment? item = await _context.Payments.Include(item=>item.User).Include(item1=>item1.PaymentPlan)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (item == null)
            {
                return new JsonResult("Payment id Not found") { StatusCode = 400 };
            }

            return Ok(item);

        }
        //method to add new the payment with parameter CreatePaymentDTO
        [HttpPost]
        public async Task<IActionResult> Add([FromForm] CreatePaymentDTO payment)
        {
            List<string> roles = new List<string>() { UserRole.ADMIN, UserRole.DRIVER, UserRole.COMPANY };
            await _userService.CheckRole(roles);


            if (!payment.Equals(null))
            {

                var userId = await _context.Users.FirstOrDefaultAsync(item => item.Id == payment.UserId);
                if (userId == null) {
                    return new JsonResult("User id Not found") { StatusCode = 400 };
                }


                var plan = await _context.PaymentPlans.FirstOrDefaultAsync(item => item.Id == payment.PlanId);
                if (plan == null)
                {
                    return new JsonResult("Payment plan id Not found") { StatusCode = 400 };
                }


                Payment item = new Payment()
                {
                    UserId = payment.UserId,
                    PlanId = payment.PlanId,
                    Amount = plan.Amount,
                    PaymentDate = DateTime.Now,
                    PaymentStatus =  Constant.PENDING,
                    PaymentType = plan.PlanType,
                    ValidFrom  =null,
                    ValidTo = null
                };

                await _context.Payments.AddAsync(item);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetById", new { item.Id }, item);
            }

            return new JsonResult("Something went wrong") { StatusCode = 500 };
        }
        //method to update the payment with parameter UpdatePaymentDTO and id 
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] UpdatePaymentDTO payment)
        {
            List<string> roles = new List<string>() { UserRole.ADMIN };
            await _userService.CheckRole(roles);

            Payment? exist = await _context.Payments.FirstOrDefaultAsync(t => t.Id == id);

            if (exist == null)
            {
                throw new ArgumentException($"Payment with ID: {id} not found");
            }

            if (ModelState.IsValid)
            {
                var userId = await _context.Users.FirstOrDefaultAsync(item => item.Id == payment.UserId);
                if (userId == null)
                {
                    return new JsonResult("User id Not found") { StatusCode = 400 };
                }

                var plan = await _context.PaymentPlans.FirstOrDefaultAsync(item => item.Id == payment.PlanId);
                if (plan == null)
                {
                    return new JsonResult("Payment plan id Not found") { StatusCode = 400 };
                }

                if (payment.PaymentStatus == "DONE")
                {
                    exist.PaymentStatus = Constant.DONE;
                    exist.ValidFrom = DateTime.Now;
                    exist.ValidTo = DateTime.Now.AddMonths(exist.PaymentType == "Month" ? 1 : 3);  
                }

                exist.Amount = plan.Amount;
                exist.PaymentDate = DateTime.Now;
                exist.PaymentType = plan.PlanType;
                exist.PlanId = payment.PlanId;
                exist.UserId = payment.UserId;
               

                await _context.SaveChangesAsync();
                return Ok(exist);
            }

            return new JsonResult("Something went wrong") { StatusCode = 500 };
        }
        //method to delete the payment with parameter id
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            List<string> roles = new List<string>() { UserRole.ADMIN };
            await _userService.CheckRole(roles);

            Payment? exist = await _context.Payments.FirstOrDefaultAsync(t => t.Id == id);

            if (exist == null) return new JsonResult($"Payment with ID: {id} not found");

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
