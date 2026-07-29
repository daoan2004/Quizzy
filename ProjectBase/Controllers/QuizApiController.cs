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
            var QuestionList = await _dataContext.QuizHandle
                .Include(p=>p.QuizBank)
                .Where(s => s.UserID == currentUserId && s.PracticeID == PracticeID)
                .ToListAsync();
            return Ok(QuestionList);
        }
        [HttpGet("loadQuestion/{questionId}")]
        public async Task<IActionResult> loadQuestion(long questionId)
        {
            if (!TryGetCurrentUserId(out var currentUserId)) return Unauthorized();
            var question = await _dataContext.QuizHandle
                .Include(p => p.QuizBank)
                .FirstOrDefaultAsync(s => s.ID == questionId && s.UserID == currentUserId);
            return question == null ? NotFound() : Ok(question);
        }
        [HttpPost("submitAnswer")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> submitAnswer([FromForm] long questionId, [FromForm] string answer, [FromForm] long PracticeID)
        {
            if (!TryGetCurrentUserId(out var currentUserId)) return Unauthorized();
            var isCorrect = 0;
            
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
            if (!await OwnsPracticeAsync(PracticeID, currentUserId)) return NotFound();
            if (question.QuizBank.Qcorrect.Equals(answer,StringComparison.OrdinalIgnoreCase)) {
                isCorrect = 1;
            }
            var sql = "UPDATE QuizHandle SET QAnswer = @QAnswer, status = 1, isCorrect = @isCorrect WHERE ID = @QuestionID;";
            using (var connection = _dataContext.Database.GetDbConnection())
            {
                await connection.OpenAsync();
                await connection.ExecuteAsync(sql, new
                {
                    QAnswer=answer,
                    isCorrect,
                    QuestionID = questionId
                });
                // Recalculate instead of incrementing so retries/double-clicks stay idempotent.
                var updatePracticeSql = @"
                    UPDATE Practice
                    SET number_correct = (
                        SELECT COUNT(*)
                        FROM QuizHandle
                        WHERE PracticeID = @PracticeID AND isCorrect = 1
                    )
                    WHERE ID = @PracticeID AND UserID = @UserID;";
                await connection.ExecuteAsync(updatePracticeSql, new
                {
                    PracticeID,
                    UserID = currentUserId
                });

            }
            return Ok();
        }
        [HttpPost("finishAttempt")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> finishAttempt(long UserID, long PracticeID)
        {
            if (!TryGetCurrentUserId(out var currentUserId)) return Unauthorized();
            if (!await OwnsPracticeAsync(PracticeID, currentUserId)) return NotFound();
            var sql = "UPDATE Practice SET  Status = 1 WHERE ID = @PracticeID;";
            using (var connection = _dataContext.Database.GetDbConnection())
            {
                await connection.OpenAsync();
                await connection.ExecuteAsync(sql, new
                {
                    PracticeID = PracticeID 
                });


            }
            return Ok();
        }

        private bool TryGetCurrentUserId(out long userId) =>
            long.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out userId);

        private Task<bool> OwnsPracticeAsync(long practiceId, long userId) =>
            _dataContext.Practice.AnyAsync(p => p.ID == practiceId && p.UserID == userId);

    }
}
