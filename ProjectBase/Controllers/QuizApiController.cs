using Microsoft.AspNetCore.Mvc;
using ProjectBase.Helpers;
using ProjectBase.Models.DAO;
using ProjectBase.Models;
using Microsoft.EntityFrameworkCore;
using Dapper;
using System.Reflection.Metadata;

namespace ProjectBase.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
            var QuestionList = _dataContext.QuizHandle.Include(p=>p.QuizBank).Where(s => s.UserID == UserID && s.PracticeID == PracticeID).ToList();
            return Ok(QuestionList);
        }
        [HttpGet("loadQuestion/{questionId}")]
        public async Task<IActionResult> loadQuestion(long questionId)
        {
            var QuestionList = _dataContext.QuizHandle.Include(p => p.QuizBank).Where(s => s.ID==questionId).FirstOrDefault();
            return Ok(QuestionList);
        }
        [HttpPost("submitAnswer")]
        public async Task<IActionResult> submitAnswer([FromForm] long questionId, [FromForm] string answer, [FromForm] long PracticeID)
        {
            var isCorrect = 0;
            
            if (_dataContext.QuizHandle.Include(p => p.QuizBank).Where(s => s.ID == questionId).FirstOrDefault().QuizBank.Qcorrect.Equals(answer,StringComparison.OrdinalIgnoreCase)) {
                isCorrect = 1;
            }
            var sql = "UPDATE QuizHandle SET QAnswer = @QAnswer, status = 1, isCorrect = @isCorrect WHERE ID = "+questionId+";";
            using (var connection = _dataContext.Database.GetDbConnection())
            {
                await connection.OpenAsync();
                await connection.ExecuteAsync(sql, new
                {
                    QAnswer=answer,
                    isCorrect,
                });
                // Update Practice table if answer is correct
                if (isCorrect == 1)
                {
                    var updatePracticeSql = "UPDATE Practice SET number_correct = number_correct + 1 WHERE ID = @PracticeID;";
                    await connection.ExecuteAsync(updatePracticeSql, new
                    {
                        PracticeID = PracticeID // Assuming `PracticeID` is the foreign key to the Practice table
                    });
                }

            }
            return Ok();
        }
        [HttpPost("finishAttempt")]
        public async Task<IActionResult> finishAttempt(long UserID, long PracticeID)
        {
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


    }
}
