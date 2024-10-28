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
            var quizhandle = await _dataContext.QuizHandle.Include(s => s.QuizBank).Where(s => s.UserID == UserID && s.PracticeID == PracticeID).ToListAsync();
            var practice = await _dataContext.Practice.Where(p => p.ID == PracticeID).FirstOrDefaultAsync();
            if (practice != null) { 
            ViewData["Number_quiz"] = practice.number_quest;
            ViewData["Level"] = _dataContext.PracticeLevel.Where(p=> p.ID == practice.levelID).FirstOrDefault().title;
            ViewData["QuizTitle"] = practice.title;
            ViewData["SubjectTitle"] = _dataContext.Subjects.Where(p => p.ID == practice.SubjectID).FirstOrDefault().title;
            }
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
