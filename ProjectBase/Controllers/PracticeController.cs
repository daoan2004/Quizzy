
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using ProjectBase.Helpers;
using ProjectBase.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ProjectBase.Controllers
{

    [Authorize]
    public class PracticeController : Controller
    {
        private readonly ILogger<PracticeController> _logger;
        private readonly DataContext _dataContext;
        public PracticeController(ILogger<PracticeController> logger, DataContext context)
        {
            _logger = logger;
            _dataContext = context;

        }

        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Details(long id)
        {
            if (!TryGetCurrentUserId(out var userId))
            {
                return Challenge();
            }

            var practice = await _dataContext.Practice
                .Include(s => s.Subject)
                .ThenInclude(sc => sc.Subject_Category)
                .ThenInclude(c => c.Category)
                .Include(l => l.Level)
                .Include(t => t.Topic)
                .FirstOrDefaultAsync(p => p.ID == id && p.UserID == userId);
            if (practice == null)
            {
                return NotFound();
            }

            return View(practice);
        }
        public IActionResult NewPractice()
        {
            return View();
        }

        private bool TryGetCurrentUserId(out long userId) =>
            long.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out userId);
    }
}
