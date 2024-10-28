
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using ProjectBase.Helpers;
using ProjectBase.Models;

namespace ProjectBase.Controllers
{

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
        public  IActionResult Details(long id)
        {
            var practice =  _dataContext.Practice
                .Include(s => s.Subject)
                .ThenInclude(sc => sc.Subject_Category)
                .ThenInclude(c => c.Category)
                .Include(l => l.Level)
                .Include(t => t.Topic)
                .Where(p=> p.ID == id).FirstOrDefault();
          
          
            return View(practice);
        }
        public IActionResult NewPractice()
        {
            return View();
        }
    }
}
