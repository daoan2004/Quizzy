using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectBase.Helpers;
using ProjectBase.Models;
using ProjectBase.Models.DAO;

namespace ProjectBase.Controllers
{
    public class QuizController : Controller
    {
        private readonly ILogger<QuizController> _logger;
        private readonly DataContext _dataContext;
        public QuizController(ILogger<QuizController> logger, DataContext context)
        {
            _logger = logger;
            _dataContext = context;

        }
        public async Task<IActionResult> HandleAsync(long UserID, long PracticeID, bool isPractice)
        {
            ViewData["UserID"] = UserID;
            ViewData["PracticeID"] = PracticeID;
            ViewData["IsPractice"] = isPractice;
            var practice = await _dataContext.Practice.FirstOrDefaultAsync(p => p.ID == PracticeID);
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
