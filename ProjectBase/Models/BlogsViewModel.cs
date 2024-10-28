using ProjectBase.Models.DAO;

namespace ProjectBase.Models
{
    public class BlogsViewModel
    { 
        public List<CategoryModel> Category { get; set; }
        public List<BlogsModel> LatestPosts { get; set; }
        public List<BlogsModel> BlogList { get; set; } 
        public List<CategoryModel> BlogCategory { get; set; }
        public BlogsModel BlogDetail { get; set; }
        public User BlogUser { get; set; }
    }
}
