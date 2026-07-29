using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectBase.Helpers;
using ProjectBase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ProjectBase.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MyRegistrationsApiController : ControllerBase
    {
        private readonly DataContext _dataContext;

        public MyRegistrationsApiController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        [HttpGet("GetAllRegistrations/{userId}")]
        public async Task<IActionResult> GetAllRegistrations(long userId, [FromQuery] long? subjectId = null, [FromQuery] string? statusId = null)
        {
            if (!TryGetCurrentUserId(out var currentUserId)) return Unauthorized();
            try
            {
                // Truy vấn cơ sở dữ liệu với các khóa ngoại được bao gồm
                var query = _dataContext.Recipe
                    .Include(r => r.Subjects)
                    .Include(r => r.PricePackage)
                    .Where(r => r.UserID == currentUserId &&
                                r.Status != RegistrationStatuses.Cancelled);

                // Áp dụng bộ lọc nếu có
                if (subjectId.HasValue)
                {
                    query = query.Where(r => r.SubjectID == subjectId);
                }

                if (!string.IsNullOrEmpty(statusId))
                {
                    query = query.Where(r => r.Status == statusId);
                }

                var registrations = await query
                    .OrderByDescending(r => r.BuyAt)
                    .Select(r => new
                    {
                        r.ID,
                        r.PricePackage_ID,
                        r.UserID,
                        r.SubjectID,
                        r.BuyAt,
                        r.EndAt,
                        r.Status,
                        r.PricePackage_Type,
                        TotalCost = r.PricePackage.SalePrice,
                        SubId = r.Subjects.ID,
                        SubjectTitle = r.Subjects.title
                    })
                    .ToListAsync();

                return Ok(registrations);
            }
            catch (Exception)
            {
                return Problem(
                    statusCode: StatusCodes.Status500InternalServerError,
                    title: "Unable to load registrations.");
            }
        }

        [HttpPost("CancelRegistration/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelRegistration(long id)
        {
            if (!TryGetCurrentUserId(out var currentUserId)) return Unauthorized();
            try
            {
                var registration = await _dataContext.Recipe
                    .FirstOrDefaultAsync(r => r.ID == id && r.UserID == currentUserId);
                if (registration == null)
                {
                    return NotFound(new { success = false, message = "Registration not found." });
                }
                if (registration.Status != RegistrationStatuses.Submitted)
                {
                    return Conflict(new
                    {
                        success = false,
                        message = "Only a submitted registration can be cancelled."
                    });
                }

                registration.Status = RegistrationStatuses.Cancelled;
                await _dataContext.SaveChangesAsync();

                return Ok(new { success = true, message = "Registration cancelled successfully." });
            }
            catch (Exception)
            {
                return Problem(
                    statusCode: StatusCodes.Status500InternalServerError,
                    title: "Unable to cancel registration.");
            }
        }

        [HttpGet("GetAllSubjects")]
        public async Task<IActionResult> GetAllSubjects()
        {
            try
            {
                var subjects = await _dataContext.Subjects.Select(s => new
                {
                    s.ID,
                    s.title
                }).ToListAsync();

                return Ok(subjects);
            }
            catch (Exception)
            {
                return Problem(
                    statusCode: StatusCodes.Status500InternalServerError,
                    title: "Unable to load subjects.");
            }
        }
        [HttpGet("GetAllStatuses")]
        public IActionResult GetAllStatuses()
        {
            try
            {
                var statuses = new[]
                {
                    RegistrationStatuses.Registered,
                    RegistrationStatuses.Submitted
                };
                return Ok(statuses);
            }
            catch (Exception)
            {
                return Problem(
                    statusCode: StatusCodes.Status500InternalServerError,
                    title: "Unable to load registration statuses.");
            }
        }

        [HttpPost("PayPackage/{registrationId}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PayPackage(long registrationId)
        {
            if (!TryGetCurrentUserId(out var currentUserId)) return Unauthorized();
            try
            {
                var registration = await _dataContext.Recipe
                    .FirstOrDefaultAsync(r => r.ID == registrationId && r.UserID == currentUserId);
                if (registration == null)
                {
                    return NotFound(new { success = false, message = "Registration not found." });
                }
                if (registration.Status != RegistrationStatuses.Submitted)
                {
                    return Conflict(new
                    {
                        success = false,
                        message = "Only a submitted registration can be paid."
                    });
                }

                registration.Status = RegistrationStatuses.Registered;
                _dataContext.Recipe.Update(registration);
                await _dataContext.SaveChangesAsync();

                return Ok(new { success = true });
            }
            catch (Exception)
            {
                return Problem(
                    statusCode: StatusCodes.Status500InternalServerError,
                    title: "Unable to update registration payment.");
            }
        }

        private bool TryGetCurrentUserId(out long userId) =>
            long.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out userId);

    }
}
