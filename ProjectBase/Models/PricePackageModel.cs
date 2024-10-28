using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Numerics;
using System.Text.Json.Serialization;

namespace ProjectBase.Models
{
    public class PricePackageModel
    {
        [Key]
        public long ID { get; set; }
        public long SubjectID { get; set; }
        public long PackageType { get; set; }
        public long ListPrice { get; set; }
        public long SalePrice { get; set; }
        [ForeignKey("SubjectID")]
        public SubjectsModel Subjects { get; set; }
        public ICollection<RecipeModel> Recipe { get; set; }
    }
}
