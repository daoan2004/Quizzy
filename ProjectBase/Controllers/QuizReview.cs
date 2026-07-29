using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectBase.Helpers;
using ProjectBase.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ProjectBase.Controllers
{
    [Authorize]
    public class QuizReview : Controller
    {

        private readonly DataContext _context;

        public QuizReview(DataContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            return RedirectToAction("Index", "Practice");
        }

        public async Task<IActionResult> Detail(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var practiceId = id.Value;
            if (!long.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var currentUserId))
            {
                return Challenge();
            }

            var practice = await _context.Practice
                .Include(p => p.User)
                .Include(p => p.Subject)
                .Include(p => p.Level)
                .Include(p => p.Topic)
                .FirstOrDefaultAsync(p => p.ID == practiceId && p.UserID == currentUserId);

            if (practice == null)
            {
                return NotFound();
            }

            var quiz_review = await _context.QuizHandle
                .Include(qh => qh.QuizBank)
                .Where(qh => qh.PracticeID == practiceId && qh.UserID == currentUserId)
                .ToListAsync();

            var model = new QuizReviewViewModel
            {
                QuizReviews = quiz_review,
                Practice = practice
            };

            // Format time fields
            ViewBag.DurationFormatted = FormatTime(practice.duration);
            ViewBag.TimeTakenFormatted = FormatTime(practice.time_taken);

            return View(model);
        }

        private string FormatTime(TimeOnly time)
        {
            // Check if the hour part is zero
            if (time.Hour == 0)
            {
                // Format as minutes and seconds with the "Minutes" label
                return $"{time.Minute:D2} Minutes";
            }
            else
            {
                // Format as hours, minutes, and seconds with the "Hour" and "Minutes" labels
                return $"{time.Hour:D1} Hour {time.Minute:D2} Minutes";
            }
        }
    }
}
