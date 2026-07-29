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
            ViewData["MetaDescription"] = "Read Quizly learning posts, study tips, and subject updates for better quiz preparation.";
            var latestCutoff = DateTime.Now.AddDays(-25);
            var category = await _dataContext.Category.ToListAsync();
            var bloglist = await _dataContext.Blogs.OrderByDescending(b => b.updatedAt).ToListAsync();
            var lastestpost = await _dataContext.Blogs
                .Where(blog => blog.updatedAt >= latestCutoff)
                .ToListAsync();
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
            ViewData["MetaDescription"] = "Read a Quizly blog article with study guidance and quiz learning insights.";
            var blogdetail = await _dataContext.Blogs.FirstOrDefaultAsync(b => b.ID == blogid);
            if (blogdetail == null)
            {
                return NotFound();
            }

            var bloguser = await _dataContext.Users.FirstOrDefaultAsync(u => u.ID == blogdetail.userID)
                ?? await _dataContext.Users.FirstOrDefaultAsync(u => u.ID == userid);
            if (bloguser == null)
            {
                return NotFound();
            }

            var latestCutoff = DateTime.Now.AddDays(-14);
            var category = await _dataContext.Category.ToListAsync();
            var lastestpost = await _dataContext.Blogs
                .Where(blog => blog.updatedAt >= latestCutoff)
                .ToListAsync();
            var blogcategory = await (from c in _dataContext.Category
                              join bc in _dataContext.Blogs_Category on c.ID equals bc.CategoryID
                              where bc.BlogID == blogid
                              select c).ToListAsync();
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
                ViewData["MetaDescription"] = $"Search Quizly blog posts matching {searchPhrase}.";
                var latestCutoff = DateTime.Now.AddDays(-14);
                var category = await _dataContext.Category.ToListAsync();
                var bloglist = await _dataContext.Blogs
                    .Where(b => b.title.Contains(searchPhrase))
                    .ToListAsync();
                var lastestpost = await _dataContext.Blogs
                    .Where(blog => blog.updatedAt >= latestCutoff)
                    .ToListAsync();
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
