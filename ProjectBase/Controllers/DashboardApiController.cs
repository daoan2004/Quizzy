using Microsoft.AspNetCore.Mvc;
using ProjectBase.Models;
using Microsoft.EntityFrameworkCore;
using ProjectBase.Helpers;

namespace ProjectBase.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardApiController : ControllerBase
    {
        private readonly DataContext _context;

        public DashboardApiController(DataContext context)
        {
            _context = context;
        }

        // Endpoint to get new registrations
        [HttpGet("registrations")]
        public async Task<IActionResult> GetNewRegistrations()
        {
            var registrations = await _context.Recipe
                .Where(r => r.Status == "Registrated" || r.Status == "Submitted" || r.Status == "Cancelled")
                .Include(r => r.Subjects)
                .Select(r => new RegistrationViewModel
                {
                    SubjectTitle = r.Subjects.title,
                    Status = r.Status,
                    RecipeID = r.ID
                })
                .ToListAsync();

            return Ok(registrations);
        }
        // GET: api/Dashboard/Subjects
        [HttpGet("Subjects")]
        public async Task<IActionResult> GetSubjects()
        {
            var subjects = await _context.Subjects
                .Select(s => new { s.ID, s.title })
                .ToListAsync();

            return Ok(subjects);
        }

        [HttpGet("total-revenue")]
        public async Task<IActionResult> GetTotalRevenue()
        {
            var recipes = await _context.Recipe
                .Include(r => r.PricePackage)
                .Where(r => r.Status == "Registrated")
                .ToListAsync();

            long totalRevenue = 0;
            foreach (var recipe in recipes)
            {
                totalRevenue += recipe.PricePackage.SalePrice;
            }

            return Ok(new { TotalRevenue = totalRevenue });
        }

        [HttpGet("RevenuesBySubject")]
        public async Task<IActionResult> GetRevenuesBySubject(int subjectId)
        {
            var revenue = await _context.Recipe
                .Where(r => r.SubjectID == subjectId && r.Status == "Registrated")
                .SumAsync(r => r.PricePackage.SalePrice);
            return Ok(new { TotalRevenue = revenue });
        }

        [HttpGet("customer-stats")]
        public async Task<IActionResult> GetCustomerStats()
        {
            var today = DateTime.UtcNow;
            var aWeekAgo = today.AddDays(-7);

            var newlyRegisteredCount = await _context.Users
                .Where(u => u.register_date >= aWeekAgo)
                .CountAsync();

            var newlyBoughtCount = await _context.Recipe
                .Where(p => p.BuyAt >= aWeekAgo)
                .CountAsync();

            return Ok(new { NewlyRegistered = newlyRegisteredCount, NewlyBought = newlyBoughtCount });
        }

        // Endpoint to get order counts, optionally filtered by date
        [HttpGet("order-count")]
        public async Task<IActionResult> GetOrderCount([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            // Lấy thông tin múi giờ của Việt Nam
            TimeZoneInfo vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"); // Mã múi giờ cho Việt Nam trên Windows
                                                                                                         // Nếu sử dụng Linux, bạn có thể cần thay thế bằng "Asia/Ho_Chi_Minh"

            if (!startDate.HasValue || !endDate.HasValue)
            {
                return BadRequest("Both start date and end date are required.");
            }

            if (endDate < startDate)
            {
                return BadRequest("End date must be greater than or equal to start date.");
            }

            var start = startDate.Value.Date;
            var end = endDate.Value.Date.AddDays(1).AddTicks(-1); // Ensure the end date is inclusive

            var orderCounts = await _context.Recipe
                .Where(o => o.BuyAt >= start && o.BuyAt <= end)
                .GroupBy(o => o.BuyAt.Date)
                .Select(group => new {
                    Date = group.Key,
                    Count = group.Count()
                })
                .OrderBy(x => x.Date)
                .ToListAsync();
            return Ok(orderCounts);
        }

        [HttpGet("registration-count")]
        public async Task<IActionResult> GetRegistrationCount([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            if (!startDate.HasValue || !endDate.HasValue)
            {
                return BadRequest("Both start date and end date are required.");
            }

            if (endDate < startDate)
            {
                return BadRequest("End date must be greater than or equal to start date.");
            }

            var start = startDate.Value.Date;
            var end = endDate.Value.Date.AddDays(1).AddTicks(-1); // Ensure the end date is inclusive

            var registrationCounts = await _context.Recipe
                .Where(o => o.Status == "Registrated" && o.BuyAt >= start && o.BuyAt <= end)
                .GroupBy(o => o.BuyAt.Date)
                .Select(group => new {
                    Date = group.Key,
                    Count = group.Count()
                    
                })
                .OrderBy(x => x.Date)
                .ToListAsync();
            
            return Ok(registrationCounts);
        }

        [HttpGet("revenues-by-subject")]
        public async Task<IActionResult> GetRevenuesByAllSubjects()
        {
            var subjectRevenues = await _context.Recipe
                .Where(r => r.Status == "Registrated")
                .GroupBy(r => r.Subjects)
                .Select(group => new {
                    SubjectName = group.Key.title,
                    Revenue = group.Sum(g => g.PricePackage.SalePrice) // Assuming SalePrice is a field in PricePackage
                })
                .OrderByDescending(sr => sr.Revenue)
                .ToListAsync();

            return Ok(subjectRevenues);
        }


    }
}
