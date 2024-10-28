using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using ProjectBase.Helpers;
using ProjectBase.Models;

namespace ProjectBase.Controllers
{
    public class BlogsController : Controller
    {
        private readonly ILogger<BlogsController> _logger;
        private readonly DataContext _dataContext;
        public BlogsController(ILogger<BlogsController> logger, DataContext context)
        {
            _logger = logger;
            _dataContext = context;

        }
        public async Task<IActionResult> Index()
        {
            var category = _dataContext.Category.ToList();
            var bloglist = _dataContext.Blogs.OrderByDescending(b => b.updatedAt).ToList();
            var lastestpost = _dataContext.Blogs.Where(blog => EF.Functions.DateDiffDay(blog.updatedAt, DateTime.Now) <= 25)
   .ToList();
            var viewModel = new BlogsViewModel
            {
                Category = category,
                BlogList = bloglist,
                LatestPosts = lastestpost

            };        
            return View(viewModel);
        }
        public async Task<IActionResult> BlogsDetail(long blogid ,long userid)
        {
            var category = _dataContext.Category.ToList();
            var lastestpost = _dataContext.Blogs.Where(blog => EF.Functions.DateDiffDay(blog.updatedAt, DateTime.Now) <= 14)
   .ToList();
            var blogdetail = _dataContext.Blogs.Where(b => b.ID == blogid).FirstOrDefault();
            var bloguser = _dataContext.Users.Where(u => u.ID == userid).FirstOrDefault();
            var blogcategory = (from c in _dataContext.Category
                              join bc in _dataContext.Blogs_Category on c.ID equals bc.CategoryID
                              where bc.BlogID == blogid
                              select c).ToList();
            var viewModel = new BlogsViewModel
            {
                Category = category,
                LatestPosts = lastestpost,
                BlogDetail = blogdetail,
                BlogUser = bloguser,
                BlogCategory = blogcategory,
            };
            return View(viewModel);
        }
        public IActionResult GetBlogCategory(int blogid)
        {
            var categories = from c in _dataContext.Category
                             join bc in _dataContext.Blogs_Category on c.ID equals bc.CategoryID
                             join b in _dataContext.Blogs on bc.BlogID equals b.ID
                             where b.ID == blogid
                             select c;
            return View(categories);
        }

        [HttpPost]
        public async Task<IActionResult> ShowSearchResults(string searchPhrase)
        {
            if (string.IsNullOrEmpty(searchPhrase))
            {
                // Handle empty search phrase
                return RedirectToAction(nameof(Index));
            }
            else {
                var category = _dataContext.Category.ToList();
                var bloglist = _dataContext.Blogs.Where(b => b.title.Contains(searchPhrase)).ToList();


                var lastestpost = _dataContext.Blogs.Where(blog => EF.Functions.DateDiffDay(blog.updatedAt, DateTime.Now) <= 14)
       .ToList();
                var viewModel = new BlogsViewModel
                {
                    Category = category,
                    BlogList = bloglist,
                    LatestPosts = lastestpost

                };

                return View("Index", viewModel);
            }
        }

    }
}
