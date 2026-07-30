using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectBase.Helpers;
using ProjectBase.Models;
using ProjectBase.Models.DAO;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ProjectBase.Controllers
{
    [Authorize]
    public class QuizController : Controller
    {
        private readonly ILogger<QuizController> _logger;
        private readonly DataContext _dataContext;
        public QuizController(ILogger<QuizController> logger, DataContext context)
        {
            _logger = logger;
            _dataContext = context;

        }
        public async Task<IActionResult> HandleAsync(long PracticeID)
        {
            if (!long.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var currentUserId))
            {
                return Challenge();
            }

            ViewData["UserID"] = currentUserId;
            ViewData["PracticeID"] = PracticeID;
            var practice = await _dataContext.Practice
                .FirstOrDefaultAsync(p => p.ID == PracticeID && p.UserID == currentUserId);
            if (practice == null)
            {
                return NotFound();
            }
            var level = await _dataContext.PracticeLevel.FirstOrDefaultAsync(p => p.ID == practice.levelID);
            var subject = await _dataContext.Subjects.FirstOrDefaultAsync(p => p.ID == practice.SubjectID);
            if (level == null || subject == null)
            {
                return NotFound();
            }
            ViewData["Number_quiz"] = practice.number_quest;
            ViewData["Level"] = level.title;
            ViewData["QuizTitle"] = practice.title;
            ViewData["SubjectTitle"] = subject.title;
            var startedAtUtc = DateTime.SpecifyKind(practice.taken_date, DateTimeKind.Utc);
            ViewData["AttemptEndsAtUtc"] = startedAtUtc
                .Add(practice.duration.ToTimeSpan())
                .ToString("O");
            var isPractice = !practice.SimulationExamID.HasValue;
            ViewData["IsPractice"] = isPractice;
            if (isPractice)
            {
                ViewData["Type"] = "Practice";
            }
            else {
                ViewData["Type"] = "Exam";
            }

            // Sử dụng dữ liệu để hiển thị hoặc xử lý

            return View(practice);
        }
        
    }
}
