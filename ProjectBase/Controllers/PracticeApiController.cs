using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProjectBase.Helpers;
using ProjectBase.Models;
using ProjectBase.Models.DAO;
using Dapper;
using Microsoft.Extensions.Logging;
namespace ProjectBase.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PracticeApiController : ControllerBase
    {
        private readonly DataContext _dataContext;

        public PracticeApiController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        [HttpGet("GetPracticePagination/{UserID}")]
        public async Task<ActionResult<IEnumerable<PracticeModel>>> GetPracticePagination(long UserID, [FromQuery] int page = 1, [FromQuery] int pageSize=5, [FromQuery] long? subjectId = null, [FromQuery] int? levelId = null)
        {
            try { 
                
                var query = _dataContext.Practice
                .Include(s => s.Subject)            
                .Include(s => s.Level).OrderByDescending(p => p.taken_date)
                .Where(u => u.UserID == UserID);
                
                if (subjectId.HasValue) {
                    query = query.Where(p => p.SubjectID == subjectId);
                }
                if (levelId.HasValue) {
                    query = query.Where(p=> p.levelID == levelId);
                }
                var practice= await query
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
                var totalItem = await query.CountAsync();
                var totalPages = (int)System.Math.Ceiling(totalItem / (double)pageSize);
                return Ok(new {practice, totalItem, totalPages,currentPage = page });
            } catch (Exception ex) { return BadRequest(ex.Message); }
        }
        [HttpGet("LoadFilter/{UserID}")]
        public async Task<ActionResult<IEnumerable<PracticeModel>>> LoadFilter(long UserID)
        {
            try
            {            
                var practice = await _dataContext.Practice
                .Include(s => s.Subject)
                .Include(s => s.Level)
                .Where(u => u.UserID == UserID)
                .ToListAsync();
                return Ok(practice);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("LoadSubject/{UserID}")]
        public async Task<ActionResult<IEnumerable<RecipeModel>>> LoadSubject(long UserID)
        {
            try
            {
                var subject = await _dataContext.Recipe
                .Include(s => s.Subjects)
                .Where (u => u.UserID == UserID).Where(r=>r.Status== "Registrated")
                .ToListAsync();
                return Ok(subject);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("AddPractice")]
        public async Task<IActionResult> AddPractice()
        {
            try
            {
                var UserID = Request.Form["UserID"];
                var SubjectID = Convert.ToInt32(Request.Form["SubjectID"]);
                var title = Request.Form["title"];
                var number_quest = Convert.ToInt32(Request.Form["number_quest"]);
                string Quest_group = Request.Form["Quest_group"];
                var duration = Request.Form["duration"];
                var levelID = Convert.ToInt32(Request.Form["levelID"]);
                var topicID = 1;
                var isMark = false;
                var status = false;
                var isCorrect = false;
                var QAnswer = "";
                var sql = "INSERT INTO Practice (UserID, SubjectID, title, number_quest, Quest_group, duration, levelID, taken_date, time_taken, Status, number_correct, topicID) " +
                                  "VALUES (@UserID, @SubjectID, @title, @number_quest, @Quest_group, @duration, @levelID, @taken_date, @time_taken, @Status, @number_correct, @topicID); "+
                                  "SELECT CAST(SCOPE_IDENTITY() AS int);";
                int PracticeID;
                using (var connection = _dataContext.Database.GetDbConnection())
                {
                    await connection.OpenAsync();
                    PracticeID = await connection.ExecuteScalarAsync<int>(sql, new
                    {
                        UserID,
                        SubjectID,
                        title,
                        number_quest,
                        Quest_group,
                        duration,
                        levelID,
                        taken_date = DateTime.Now,
                        time_taken = "00:00:00",
                        Status = false,
                        number_correct = 0,
                        topicID,
                    });
                

                    if (levelID == 1) {
                        int numberQuestLevel1 = (int)(number_quest * 0.9);
                        int numberQuestLevel2 = number_quest - numberQuestLevel1;
                        var queryquizLevel1 = _dataContext.QuizBank
                                        .Where(q => q.SubjectID == SubjectID && q.TopicID == topicID && q.LevelID == 1);
                        var queryquizLevel2 = _dataContext.QuizBank
                                        .Where(q => q.SubjectID == SubjectID && q.TopicID == topicID && q.LevelID == 2);
                        if (!Quest_group.Equals("0")) {
                            queryquizLevel1 = queryquizLevel1.Where(q => q.GroupID.Equals(Quest_group));
                            queryquizLevel2 = queryquizLevel2.Where(q => q.GroupID.Equals(Quest_group));
                        }                
                        var quizLevel1 = queryquizLevel1
                                        .Take(numberQuestLevel1)
                                        .ToList();
                        var quizLevel2 = queryquizLevel2
                                        .Take(numberQuestLevel2)
                                        .ToList();
                        
                        var listQuiz = new List<long>();
                        foreach (var item in quizLevel1) {
                            listQuiz.Add(item.ID);
                        }
                        foreach (var item in quizLevel2)
                        {
                            listQuiz.Add(item.ID);
                        }
                        var random = new Random();
                        listQuiz = listQuiz.OrderBy(x => random.Next()).ToList();

                        var addHandle = "INSERT INTO QuizHandle (UserID, PracticeID, QuizID, QAnswer, isMark, status, isCorrect) " +
                                      "VALUES (@UserID, @PracticeID, @QuizID, @QAnswer, @isMark, @status, @isCorrect)";
                    
                            foreach (var QuizID in listQuiz)
                            {
                        
                                await connection.ExecuteAsync(addHandle, new
                                {
                                    UserID,
                                    PracticeID,
                                    QuizID,
                                    QAnswer,
                                    isMark,
                                    status,
                                    isCorrect,
                                });
                            }
                    
                    } 
                
                else if (levelID == 2)
                {
                    int numberQuestLevel1 = (int)(number_quest * 0.3);
                    int numberQuestLevel2 = number_quest - numberQuestLevel1;
                    var queryquizLevel1 = _dataContext.QuizBank
                                    .Where(q => q.SubjectID == SubjectID && q.TopicID == topicID && q.LevelID == 1);
                    var queryquizLevel2 = _dataContext.QuizBank
                                    .Where(q => q.SubjectID == SubjectID && q.TopicID == topicID && q.LevelID == 2);
                    if (!Quest_group.Equals("0"))
                    {
                        queryquizLevel1 = queryquizLevel1.Where(q => q.GroupID.Equals(Quest_group));
                        queryquizLevel2 = queryquizLevel2.Where(q => q.GroupID.Equals(Quest_group));
                    }
                    var quizLevel1 = queryquizLevel1.OrderBy(r => Guid.NewGuid())
                                    .Take(numberQuestLevel1)
                                    .ToList();
                    var quizLevel2 = queryquizLevel2.OrderBy(r => Guid.NewGuid())
                                    .Take(numberQuestLevel2)
                                    .ToList();
                    var listQuiz = new List<long>();
                    foreach (var item in quizLevel1)
                    {
                        listQuiz.Add(item.ID);
                    }
                    foreach (var item in quizLevel2)
                    {
                        listQuiz.Add(item.ID);
                    }
                    var random = new Random();
                    listQuiz = listQuiz.OrderBy(x => random.Next()).ToList();
                        var addHandle = "INSERT INTO QuizHandle (UserID, PracticeID, QuizID, QAnswer, isMark, status, isCorrect) " +
                                      "VALUES (@UserID, @PracticeID, @QuizID, @QAnswer, @isMark, @status, @isCorrect)";

                        foreach (var QuizID in listQuiz)
                        {

                            await connection.ExecuteAsync(addHandle, new
                            {
                                UserID,
                                PracticeID,
                                QuizID,
                                QAnswer,
                                isMark,
                                status,
                                isCorrect,
                            });
                        }

                    }
                else if (levelID == 3)
                {
                    int numberQuestLevel2 = (int)(number_quest * 0.6);
                    int numberQuestLevel3 = number_quest - numberQuestLevel2;
                    var queryquizLevel2 = _dataContext.QuizBank
                                    .Where(q => q.SubjectID == SubjectID && q.TopicID == topicID && q.LevelID == 2);
                    var queryquizLevel3 = _dataContext.QuizBank
                                    .Where(q => q.SubjectID == SubjectID && q.TopicID == topicID && q.LevelID == 3);
                    if (!Quest_group.Equals("0"))
                    {
                        queryquizLevel2 = queryquizLevel2.Where(q => q.GroupID.Equals(Quest_group));
                        queryquizLevel3 = queryquizLevel3.Where(q => q.GroupID.Equals(Quest_group));
                    }
                    var quizLevel2 = queryquizLevel2.OrderBy(r => Guid.NewGuid())
                                    .Take(numberQuestLevel2)
                                    .ToList();
                    var quizLevel3 = queryquizLevel3.OrderBy(r => Guid.NewGuid())
                                    .Take(numberQuestLevel3)
                                    .ToList();
                    var listQuiz = new List<long>();
                    foreach (var item in quizLevel2)
                    {
                        listQuiz.Add(item.ID);
                    }
                    foreach (var item in quizLevel3)
                    {
                        listQuiz.Add(item.ID);
                    }
                    var random = new Random();
                    listQuiz = listQuiz.OrderBy(x => random.Next()).ToList();
                        var addHandle = "INSERT INTO QuizHandle (UserID, PracticeID, QuizID, QAnswer, isMark, status, isCorrect) " +
                                      "VALUES (@UserID, @PracticeID, @QuizID, @QAnswer, @isMark, @status, @isCorrect)";

                        foreach (var QuizID in listQuiz)
                        {

                            await connection.ExecuteAsync(addHandle, new
                            {
                                UserID,
                                PracticeID,
                                QuizID,
                                QAnswer,
                                isMark,
                                status,
                                isCorrect,
                            });
                        }

                    }
            
                }
                // Trả về phản hồi thành công mà không chuyển hướng
                return Ok(PracticeID);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return BadRequest(ex.Message);
            }
        }

    }
}
