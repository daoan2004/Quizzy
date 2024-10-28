using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectBase.Helpers;

namespace ProjectBase.Controllers
{
    public class MyRegistrationsController : Controller
    {
        private readonly ILogger<MyRegistrationsController> _logger;
        private readonly DataContext _dataContext;
        public MyRegistrationsController(ILogger<MyRegistrationsController> logger, DataContext context)
        {
            _logger = logger;
            _dataContext = context;

        }
        public async Task<IActionResult> Index()
        {
            var subjects = await _dataContext.Subjects
            .Include(s => s.Subject_Category)
            .ThenInclude(sc => sc.Category)
            .Include(s => s.Price_package)
            .ToListAsync();

            // Get the first three subjects
            var featuredSubjects = subjects.Take(3).ToList();

            var categories = await _dataContext.Category.ToListAsync();

            // Pass subjects and categories to the view
            ViewData["Categories"] = categories;
            ViewData["FeaturedSubjects"] = featuredSubjects;

            return View(subjects);
        }
    }
}
