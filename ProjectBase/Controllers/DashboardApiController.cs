using Microsoft.AspNetCore.Mvc;
using ProjectBase.Models;
using Microsoft.EntityFrameworkCore;
using ProjectBase.Helpers;

namespace ProjectBase.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Microsoft.AspNetCore.Authorization.Authorize(Policy = "DashboardAccess")]
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
                .Where(r => r.Status == RegistrationStatuses.Registered ||
                            r.Status == RegistrationStatuses.Submitted ||
                            r.Status == RegistrationStatuses.Cancelled)
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
            var totalRevenue = await _context.Recipe
                .Where(r => r.Status == RegistrationStatuses.Registered)
                .SumAsync(r => (long?)r.PricePackage.SalePrice) ?? 0;

            return Ok(new { TotalRevenue = totalRevenue });
        }

        [HttpGet("RevenuesBySubject")]
        public async Task<IActionResult> GetRevenuesBySubject(long subjectId)
        {
            var revenue = await _context.Recipe
                .Where(r => r.SubjectID == subjectId &&
                            r.Status == RegistrationStatuses.Registered)
                .SumAsync(r => (long?)r.PricePackage.SalePrice) ?? 0;
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
        public async Task<IActionResult> GetOrderCount(
            [FromQuery] DateTimeOffset? startDate,
            [FromQuery] DateTimeOffset? endDate)
        {
            if (!TryGetUtcDateRange(startDate, endDate, out var startUtc, out var endUtcExclusive, out var error))
            {
                return BadRequest(error);
            }

            var orderCounts = await _context.Recipe
                .Where(o => o.BuyAt >= startUtc && o.BuyAt < endUtcExclusive)
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
        public async Task<IActionResult> GetRegistrationCount(
            [FromQuery] DateTimeOffset? startDate,
            [FromQuery] DateTimeOffset? endDate)
        {
            if (!TryGetUtcDateRange(startDate, endDate, out var startUtc, out var endUtcExclusive, out var error))
            {
                return BadRequest(error);
            }

            var registrationCounts = await _context.Recipe
                .Where(o => o.Status == RegistrationStatuses.Registered &&
                            o.BuyAt >= startUtc && o.BuyAt < endUtcExclusive)
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
                .Where(r => r.Status == RegistrationStatuses.Registered)
                .GroupBy(r => new { r.SubjectID, r.Subjects.title })
                .Select(group => new {
                    SubjectName = group.Key.title,
                    Revenue = group.Sum(g => g.PricePackage.SalePrice)
                })
                .OrderByDescending(sr => sr.Revenue)
                .ToListAsync();

            return Ok(subjectRevenues);
        }

        private static bool TryGetUtcDateRange(
            DateTimeOffset? startDate,
            DateTimeOffset? endDate,
            out DateTime startUtc,
            out DateTime endUtcExclusive,
            out string error)
        {
            startUtc = default;
            endUtcExclusive = default;
            error = string.Empty;

            if (!startDate.HasValue || !endDate.HasValue)
            {
                error = "Both start date and end date are required.";
                return false;
            }

            startUtc = startDate.Value.UtcDateTime.Date;
            var endUtc = endDate.Value.UtcDateTime.Date;
            if (endUtc < startUtc)
            {
                error = "End date must be greater than or equal to start date.";
                return false;
            }

            if ((endUtc - startUtc).TotalDays > 366)
            {
                error = "Date range cannot exceed 366 days.";
                return false;
            }

            endUtcExclusive = endUtc.AddDays(1);
            return true;
        }

    }
}
