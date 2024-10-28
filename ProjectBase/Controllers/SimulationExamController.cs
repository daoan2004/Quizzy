using Microsoft.AspNetCore.Mvc;
using ProjectBase.Helpers;

namespace ProjectBase.Controllers
{
    public class SimulationExamController : Controller
    {
        private readonly ILogger<SimulationExamController> _logger;
        private readonly DataContext _dataContext;
        public SimulationExamController(ILogger<SimulationExamController> logger, DataContext context)
        {
            _logger = logger;
            _dataContext = context;

        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
