using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectBase.Models
{
    public class CategoryModel
    {
        public long ID { get; set; }
        public string title { get; set; }
        public string description { get; set; }

        public ICollection<Subject_CategoryModel> Subject_Category { get; set; }
    }
}
