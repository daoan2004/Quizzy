using Microsoft.AspNetCore.Mvc;
using ProjectBase.Helpers;
using ProjectBase.Models;
using ProjectBase.Models.DAO;

namespace ProjectBase.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogApiController : ControllerBase
    {
        private readonly DataContext _dataContext;

        public BlogApiController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        [HttpGet("GetBlogCategory/{blogId}")]
        public ActionResult<List<CategoryModel>> GetBlogCategory(int blogId)
        {
            var categories = (from c in _dataContext.Category
                              join bc in _dataContext.Blogs_Category on c.ID equals bc.CategoryID
                              join b in _dataContext.Blogs on bc.BlogID equals b.ID
                              where b.ID == blogId
                              select c).ToList();

            return Ok(categories);
        }
        [HttpGet("GetBlogUser/{userId}")]
        public ActionResult<List<User>> GetBlogUser(int userId)
        {
            var user = (from u in _dataContext.Users                             
                              where u.ID == userId
                              select u).ToList();

            return Ok(user);
        }
    }
}

