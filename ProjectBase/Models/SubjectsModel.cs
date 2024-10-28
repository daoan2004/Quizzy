using System.Reflection;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace ProjectBase.Models
    {
    [Table("Subjects")]
    [PrimaryKey("ID")]
    public class SubjectsModel
    {
        
        public long ID { get; set; }
        public long? UserID { get; set; }    
        public string title { get; set; }
        public string brief_info { get; set; }
        public string Description { get; set; }
        public int rate { get; set; }        
        public bool? isHot { get; set; }        
        public string? thumbnail_color { get; set; }
        public DateTime? registerDate { get; set; }

        public ICollection<Subject_CategoryModel> Subject_Category { get; set; }
        [JsonIgnore]
        public ICollection<PricePackageModel> Price_package { get; set; }
        [JsonIgnore]
        public ICollection<PracticeModel> Practice { get; set; }
        [JsonIgnore]
        public ICollection<RecipeModel> Recipes { get; set; }        
        public ICollection<SimulationExam> Exams { get; set; }


    }
}
