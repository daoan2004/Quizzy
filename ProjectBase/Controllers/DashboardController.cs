using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectBase.Helpers;

namespace ProjectBase.Controllers
{
    public class DashboardController : Controller
    {
        private readonly DataContext _dataContext;

        public DashboardController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var currentDate = DateTime.UtcNow;
            var sevenDaysAgo = currentDate.AddDays(-7);

            var totalSubjects = await _dataContext.Subjects.CountAsync();
            var newSubjects = await _dataContext.Subjects
                                                .Where(s => s.registerDate >= sevenDaysAgo)
                                                .CountAsync();
            
            ViewData["TotalSubjects"] = totalSubjects;
            ViewData["NewSubjects"] = newSubjects;

            return View();
        }

    }
}
