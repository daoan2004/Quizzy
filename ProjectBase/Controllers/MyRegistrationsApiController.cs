using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectBase.Helpers;
using ProjectBase.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectBase.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
            try
            {
                // Truy vấn cơ sở dữ liệu với các khóa ngoại được bao gồm
                var query = _dataContext.Recipe
                    .Include(r => r.Subjects)
                    .Include(r => r.PricePackage)
                    .Where(r => r.UserID == userId && r.Status != "Cancelled");

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

                // Lọc lại TotalCost bằng cách so sánh SubjectID và PricePackageType
                var filteredRegistrations = registrations.Where(r =>
                    _dataContext.Price_package.Any(pp => pp.SubjectID == r.SubjectID && pp.PackageType == r.PricePackage_Type))
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
                        TotalCost = _dataContext.Price_package
                            .Where(pp => pp.SubjectID == r.SubjectID && pp.PackageType == r.PricePackage_Type)
                            .Select(pp => pp.SalePrice)
                            .FirstOrDefault(),
                        r.SubId,
                        r.SubjectTitle
                    }).ToList();

                return Ok(filteredRegistrations);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("CancelRegistration/{id}")]
        public async Task<IActionResult> CancelRegistration(long id)
        {
            try
            {
                var registration = await _dataContext.Recipe.FindAsync(id);
                if (registration == null)
                {
                    return NotFound(new { success = false, message = "Registration not found." });
                }

                // Update registration status to "Cancelled" instead of removing it
                registration.Status = "Cancelled";
                await _dataContext.SaveChangesAsync();

                return Ok(new { success = true, message = "Registration cancelled successfully." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
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
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpGet("GetAllStatuses")]
        public IActionResult GetAllStatuses()
        {
            try
            {
                var statuses = new string[] { "Registered", "Submitted"};  // Điều chỉnh theo nhu cầu thực tế của bạn
                return Ok(statuses);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Unable to retrieve statuses: " + ex.Message });
            }
        }

        [HttpPost("PayPackage/{registrationId}")]
        public async Task<IActionResult> PayPackage(long registrationId)
        {
            try
            {
                var registration = await _dataContext.Recipe.FindAsync(registrationId);
                if (registration == null)
                {
                    return NotFound(new { success = false, message = "Registration not found." });
                }

                // Thay đổi trạng thái đăng ký
                registration.Status = "Registrated";
                _dataContext.Recipe.Update(registration);
                await _dataContext.SaveChangesAsync();

                return Ok(new { success = true });
            }
            catch (Exception ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
        }


    }
}
