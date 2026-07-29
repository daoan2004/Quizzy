using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectBase.Helpers;
using ProjectBase.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ProjectBase.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class SimulationExamApiController : ControllerBase
    {
        private readonly DataContext _dataContext;

        public SimulationExamApiController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        [HttpGet("LoadExam/{UserID}")]
        public async Task<ActionResult<IEnumerable<PracticeModel>>> LoadExam(long UserID)
        {
            if (!TryGetCurrentUserId(out var currentUserId)) return Unauthorized();
            try
            {
                var exam = await _dataContext.Recipe
                .Include(s => s.Subjects)
                .ThenInclude(e=>e.Exams)
                .Where(u => u.UserID == currentUserId).Where(r => r.Status == "Registrated")
                .ToListAsync();
                return Ok(exam);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("GetExamPagination/{UserID}")]
        public async Task<ActionResult<IEnumerable<PracticeModel>>> GetExamPagination(long UserID, [FromQuery] int page = 1, [FromQuery] int pageSize = 5, [FromQuery] long? levelId = null)
        {
            if (!TryGetCurrentUserId(out var currentUserId)) return Unauthorized();
            try
            {
                var query = _dataContext.Recipe
                .Include(s => s.Subjects)
                .ThenInclude(e=>e.Exams).ThenInclude(l=>l.Level)
               
                .Where(u => u.UserID == currentUserId);

                if (levelId.HasValue)
                {
                    
                }
               
                var practice = await query
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToListAsync();
                var totalItem = await query.CountAsync();
                var totalPages = (int)System.Math.Ceiling(totalItem / (double)pageSize);
                return Ok(new { practice, totalItem, totalPages, currentPage = page });
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpGet("LoadFilter/{UserID}")]
        public async Task<ActionResult<IEnumerable<RecipeModel>>> LoadFilter(long UserID)
        {
            if (!TryGetCurrentUserId(out var currentUserId)) return Unauthorized();
            try
            {
                var practice = await _dataContext.Recipe
                .Include(s => s.Subjects)
                .ThenInclude(e => e.Exams).ThenInclude(l => l.Level)
                .Where(u => u.UserID == currentUserId)
                .ToListAsync();
                return Ok(practice);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private bool TryGetCurrentUserId(out long userId) =>
            long.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out userId);
    }
}
