using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectBase.Helpers;
using ProjectBase.Models;


namespace ProjectBase.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DataContext _dataContext;
        public HomeController(ILogger<HomeController> logger,DataContext context)
        {          
            _logger = logger;
            _dataContext = context;

        }

        public IActionResult Index()
        {
            ViewData["MetaDescription"] = "Quizly home page with featured subjects, latest learning posts, and quiz practice recommendations.";
            var slider = _dataContext.Slider.Include(s => s.User).ToList();
            var latestPosts = _dataContext.Blogs
                .OrderByDescending(blog => blog.updatedAt)
                .Take(5)
                .ToList();
            var hotsubject = _dataContext.Subjects.Where(subject => subject.isHot == true).ToList();
            var blogview = _dataContext.Blogs.ToList();
            var subject = _dataContext.Subjects.Include(s => s.Subject_Category).ThenInclude(sc => sc.Category)
                .Include(s => s.Price_package).ToList();
            var featuredSubjects = subject.Take(3).ToList();
            ViewData["FeaturedSubjects"] = featuredSubjects;
            var viewModel = new HomeViewModel
            {
                Sliders = slider,
                LatestPosts = latestPosts,
                HotSubjects = hotsubject,
                BlogView = blogview,
                Subjects = subject,

            };

            return View(viewModel);
        }
        
        public IActionResult Privacy()
        {
            ViewData["MetaDescription"] = "Learn more about Quizly and the online quiz learning experience.";
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        
        public IActionResult Profile()
        {
            return View();
        }


        /*public IActionResult Login()
        {
            return View();
        }
        public IActionResult Register()
        {
            return View();
        }*/
        public IActionResult Blogs()
        {
            return View();
        }

        public IActionResult BlogsDetail()
        {
            return View();
        }


        

    }
}
