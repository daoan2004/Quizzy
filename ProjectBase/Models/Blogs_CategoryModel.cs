using Microsoft.EntityFrameworkCore;

namespace ProjectBase.Models
{
    [Keyless]
    public class Blogs_CategoryModel
    {
        public long BlogID { get; set; }
        public long CategoryID { get; set; }
    }
    
}
