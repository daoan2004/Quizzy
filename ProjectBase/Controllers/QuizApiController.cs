using Microsoft.AspNetCore.Mvc;
using ProjectBase.Helpers;
using ProjectBase.Models.DAO;
using ProjectBase.Models;
using Microsoft.EntityFrameworkCore;
using Dapper;
using System.Reflection.Metadata;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ProjectBase.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class QuizApiController : ControllerBase
    {
        private readonly DataContext _dataContext;

        public QuizApiController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        [HttpGet("getQuestionsList")]
        public async Task<IActionResult> getQuestionsList(long UserID, long PracticeID)
        {
            if (!TryGetCurrentUserId(out var currentUserId)) return Unauthorized();
            if (!await OwnsPracticeAsync(PracticeID, currentUserId)) return NotFound();
            var questionList = await _dataContext.QuizHandle
                .Where(s => s.UserID == currentUserId && s.PracticeID == PracticeID)
                .OrderBy(s => s.ID)
                .Select(s => new
                {
                    s.ID,
                    s.status,
                    s.isMark
                })
                .ToListAsync();
            return Ok(questionList);
        }
        [HttpGet("loadQuestion/{questionId}")]
        public async Task<IActionResult> loadQuestion(long questionId)
        {
            if (!TryGetCurrentUserId(out var currentUserId)) return Unauthorized();
            var question = await _dataContext.QuizHandle
                .Include(s => s.QuizBank)
                .FirstOrDefaultAsync(s => s.ID == questionId && s.UserID == currentUserId);
            if (question?.QuizBank == null) return NotFound();
            return Ok(new
            {
                question.ID,
                question.PracticeID,
                question.QAnswer,
                question.isMark,
                question.status,
                QuizBank = new
                {
                    question.QuizBank.ID,
                    question.QuizBank.GroupID,
                    question.QuizBank.Title,
                    question.QuizBank.QA,
                    question.QuizBank.QB,
                    question.QuizBank.QC,
                    question.QuizBank.QD,
                    question.QuizBank.QE,
                    question.QuizBank.QF,
                    SelectionLimit = question.QuizBank.Qcorrect
                        .Split(';', StringSplitOptions.RemoveEmptyEntries).Length
                }
            });
        }
        [HttpPost("submitAnswer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> submitAnswer([FromForm] long questionId, [FromForm] string answer, [FromForm] long PracticeID)
        {
            if (!TryGetCurrentUserId(out var currentUserId)) return Unauthorized();
            var question = await _dataContext.QuizHandle
                .Include(p => p.QuizBank)
                .FirstOrDefaultAsync(s => s.ID == questionId && s.UserID == currentUserId);
            if (question?.QuizBank == null)
            {
                return NotFound();
            }
            if (question.PracticeID != PracticeID)
            {
                return BadRequest("Question does not belong to the supplied practice.");
            }
            var practice = await _dataContext.Practice.FirstOrDefaultAsync(
                candidate =>
                    candidate.ID == PracticeID &&
                    candidate.UserID == currentUserId);
            if (practice == null) return NotFound();
            var attemptStateError = await ValidateAttemptCanChangeAsync(practice);
            if (attemptStateError != null) return attemptStateError;
            if (string.IsNullOrWhiteSpace(answer))
            {
                return BadRequest(new { message = "Answer is required." });
            }

            question.QAnswer = answer.Trim();
            question.status = true;
            question.isCorrect = question.QuizBank.Qcorrect.Equals(
                question.QAnswer,
                StringComparison.OrdinalIgnoreCase);
            await _dataContext.SaveChangesAsync(HttpContext.RequestAborted);
            practice.number_correct = await _dataContext.QuizHandle.CountAsync(
                handle =>
                    handle.PracticeID == PracticeID &&
                    handle.UserID == currentUserId &&
                    handle.isCorrect);
            await _dataContext.SaveChangesAsync(HttpContext.RequestAborted);
            return Ok();
        }
        [HttpPost("finishAttempt")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> finishAttempt(long UserID, long PracticeID)
        {
            if (!TryGetCurrentUserId(out var currentUserId)) return Unauthorized();
            var practice = await _dataContext.Practice.FirstOrDefaultAsync(
                candidate =>
                    candidate.ID == PracticeID &&
                    candidate.UserID == currentUserId);
            if (practice == null) return NotFound();
            if (practice.Status)
            {
                return Conflict(new
                {
                    message = "This attempt has already been submitted."
                });
            }
            practice.Status = true;
            await _dataContext.SaveChangesAsync(HttpContext.RequestAborted);
            return Ok();
        }

        [HttpPost("toggleMark")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleMark(
            [FromForm] long questionId,
            [FromForm] long PracticeID,
            [FromForm] bool isMarked)
        {
            if (!TryGetCurrentUserId(out var currentUserId)) return Unauthorized();
            var practice = await _dataContext.Practice.FirstOrDefaultAsync(
                candidate =>
                    candidate.ID == PracticeID &&
                    candidate.UserID == currentUserId);
            if (practice == null) return NotFound();
            var attemptStateError = await ValidateAttemptCanChangeAsync(practice);
            if (attemptStateError != null) return attemptStateError;

            var question = await _dataContext.QuizHandle.FirstOrDefaultAsync(handle =>
                handle.ID == questionId &&
                handle.PracticeID == PracticeID &&
                handle.UserID == currentUserId);
            if (question == null) return NotFound();
            question.isMark = isMarked;
            await _dataContext.SaveChangesAsync(HttpContext.RequestAborted);
            return Ok(new { question.isMark });
        }

        private bool TryGetCurrentUserId(out long userId) =>
            long.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out userId);

        private Task<bool> OwnsPracticeAsync(long practiceId, long userId) =>
            _dataContext.Practice.AnyAsync(p => p.ID == practiceId && p.UserID == userId);

        private async Task<IActionResult?> ValidateAttemptCanChangeAsync(
            PracticeModel practice)
        {
            if (practice.Status)
            {
                return Conflict(new { message = "This attempt has already been submitted." });
            }
            if (DateTime.UtcNow > practice.taken_date.Add(practice.duration.ToTimeSpan()))
            {
                practice.Status = true;
                await _dataContext.SaveChangesAsync(HttpContext.RequestAborted);
                return StatusCode(
                    StatusCodes.Status410Gone,
                    new { message = "The time limit for this attempt has expired." });
            }
            return null;
        }

    }
}
