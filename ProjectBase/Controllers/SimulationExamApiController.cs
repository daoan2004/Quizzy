using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectBase.Helpers;
using ProjectBase.Models;

namespace ProjectBase.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public sealed class SimulationExamApiController : ControllerBase
{
    private const int GeneralTopicId = 1;
    private readonly DataContext _context;
    private readonly ILogger<SimulationExamApiController> _logger;

    public SimulationExamApiController(
        DataContext context,
        ILogger<SimulationExamApiController> logger)
    {
        _context = context;
        _logger = logger;
    }

    [HttpGet("GetExamPagination/{userId:long}")]
    public async Task<IActionResult> GetExamPagination(
        long userId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 5,
        [FromQuery] long? levelId = null)
    {
        if (!TryGetCurrentUserId(out var currentUserId))
        {
            return Unauthorized();
        }

        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 1, 25);
        var now = DateTime.UtcNow;

        var query = _context.SimulationExam
            .AsNoTracking()
            .Include(exam => exam.Level)
            .Include(exam => exam.Subjects)
            .Where(exam => _context.Recipe.Any(registration =>
                registration.UserID == currentUserId &&
                registration.SubjectID == exam.SubjectID &&
                registration.Status == RegistrationStatuses.Registered &&
                registration.EndAt >= now));

        if (levelId.HasValue)
        {
            query = query.Where(exam => exam.LevelID == levelId.Value);
        }

        var totalItems = await query.CountAsync(HttpContext.RequestAborted);
        var exams = await query
            .OrderBy(exam => exam.ID)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(exam => new
            {
                exam.ID,
                exam.SubjectID,
                SubjectTitle = exam.Subjects.title,
                exam.ExamName,
                exam.LevelID,
                LevelTitle = exam.Level.title,
                exam.Number_Question,
                exam.Duration,
                exam.Passrate
            })
            .ToListAsync(HttpContext.RequestAborted);

        return Ok(new
        {
            exams,
            totalItems,
            totalPages = (int)Math.Ceiling(totalItems / (double)pageSize),
            currentPage = page
        });
    }

    [HttpGet("LoadFilter/{userId:long}")]
    public async Task<IActionResult> LoadFilter(long userId)
    {
        if (!TryGetCurrentUserId(out var currentUserId))
        {
            return Unauthorized();
        }

        var now = DateTime.UtcNow;
        var levels = await _context.SimulationExam
            .AsNoTracking()
            .Where(exam => _context.Recipe.Any(registration =>
                registration.UserID == currentUserId &&
                registration.SubjectID == exam.SubjectID &&
                registration.Status == RegistrationStatuses.Registered &&
                registration.EndAt >= now))
            .Select(exam => new
            {
                exam.LevelID,
                Title = exam.Level.title
            })
            .Distinct()
            .OrderBy(level => level.LevelID)
            .ToListAsync(HttpContext.RequestAborted);

        return Ok(levels);
    }

    [HttpPost("Start/{examId:long}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Start(long examId)
    {
        if (!TryGetCurrentUserId(out var currentUserId))
        {
            return Unauthorized();
        }

        var exam = await _context.SimulationExam
            .AsNoTracking()
            .FirstOrDefaultAsync(
                candidate => candidate.ID == examId,
                HttpContext.RequestAborted);
        if (exam is null)
        {
            return NotFound(new { message = "Simulation exam not found." });
        }

        var now = DateTime.UtcNow;
        var hasActiveRegistration = await _context.Recipe.AnyAsync(
            registration =>
                registration.UserID == currentUserId &&
                registration.SubjectID == exam.SubjectID &&
                registration.Status == RegistrationStatuses.Registered &&
                registration.EndAt >= now,
            HttpContext.RequestAborted);
        if (!hasActiveRegistration)
        {
            return StatusCode(
                StatusCodes.Status403Forbidden,
                new { message = "An active subject registration is required." });
        }

        var openAttempt = await _context.Practice
            .AsNoTracking()
            .FirstOrDefaultAsync(
                practice =>
                    practice.UserID == currentUserId &&
                    practice.SimulationExamID == exam.ID &&
                    !practice.Status,
                HttpContext.RequestAborted);
        if (openAttempt is not null)
        {
            return Ok(new
            {
                practiceId = openAttempt.ID,
                resumed = true
            });
        }

        var candidates = await _context.QuizBank
            .AsNoTracking()
            .Where(question =>
                question.SubjectID == exam.SubjectID &&
                question.TopicID == GeneralTopicId &&
                question.Status)
            .ToListAsync(HttpContext.RequestAborted);
        var selectedQuestions = SelectQuestions(
            candidates,
            exam.LevelID,
            exam.Number_Question);
        if (selectedQuestions.Count != exam.Number_Question)
        {
            return Conflict(new
            {
                message =
                    $"Not enough questions for this exam. Required " +
                    $"{exam.Number_Question}, available {selectedQuestions.Count}."
            });
        }

        await using var transaction = _context.Database.IsRelational()
            ? await _context.Database.BeginTransactionAsync(
                HttpContext.RequestAborted)
            : null;
        try
        {
            var practice = new PracticeModel
            {
                UserID = currentUserId,
                SubjectID = exam.SubjectID,
                SimulationExamID = exam.ID,
                title = exam.ExamName,
                number_quest = exam.Number_Question,
                Quest_group = "0",
                duration = TimeOnly.FromTimeSpan(
                    TimeSpan.FromMinutes(exam.Duration)),
                levelID = exam.LevelID,
                taken_date = now,
                time_taken = TimeOnly.MinValue,
                Status = false,
                number_correct = 0,
                topicID = GeneralTopicId
            };
            _context.Practice.Add(practice);
            await _context.SaveChangesAsync(HttpContext.RequestAborted);

            _context.QuizHandle.AddRange(selectedQuestions.Select(question =>
                new QuizHandleModel
                {
                    UserID = currentUserId,
                    PracticeID = practice.ID,
                    QuizID = question.ID,
                    QAnswer = string.Empty,
                    isMark = false,
                    status = false,
                    isCorrect = false
                }));
            await _context.SaveChangesAsync(HttpContext.RequestAborted);
            if (transaction is not null)
            {
                await transaction.CommitAsync(HttpContext.RequestAborted);
            }

            return Ok(new
            {
                practiceId = practice.ID,
                resumed = false
            });
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Simulation exam {ExamId} start failed for user {UserId}. TraceId: {TraceId}",
                exam.ID,
                currentUserId,
                HttpContext.TraceIdentifier);
            return Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: "Unable to start simulation exam.");
        }
    }

    private static List<QuizBankModel> SelectQuestions(
        IReadOnlyCollection<QuizBankModel> candidates,
        int levelId,
        int questionCount)
    {
        var distribution = levelId switch
        {
            1 => new Dictionary<int, int>
            {
                [1] = (int)(questionCount * 0.9),
                [2] = questionCount - (int)(questionCount * 0.9)
            },
            2 => new Dictionary<int, int>
            {
                [1] = (int)(questionCount * 0.3),
                [2] = questionCount - (int)(questionCount * 0.3)
            },
            3 => new Dictionary<int, int>
            {
                [2] = (int)(questionCount * 0.6),
                [3] = questionCount - (int)(questionCount * 0.6)
            },
            _ => []
        };

        var selected = new List<QuizBankModel>(questionCount);
        foreach (var requirement in distribution)
        {
            selected.AddRange(candidates
                .Where(question => question.LevelID == requirement.Key)
                .OrderBy(_ => Random.Shared.Next())
                .Take(requirement.Value));
        }

        return selected;
    }

    private bool TryGetCurrentUserId(out long userId) =>
        long.TryParse(
            User.FindFirstValue(ClaimTypes.NameIdentifier),
            out userId);
}
