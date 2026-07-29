using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectBase.Models
{
    public class CategoryModel
    {
        public long ID { get; set; }
        public string title { get; set; } = null!;
        public string description { get; set; } = null!;

        public ICollection<Subject_CategoryModel> Subject_Category { get; set; } = [];
    }
}
