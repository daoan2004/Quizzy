using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectBase.Models
{
    [Keyless]
    public class Subject_CategoryModel
    {
        public long SubjectID { get; set; }
        public long CategoryID { get; set; }

        [ForeignKey("CategoryID")]
        public CategoryModel Category { get; set; } = null!;

        [ForeignKey("SubjectID")]
        public SubjectsModel Subjects { get; set; } = null!;
    }
}
