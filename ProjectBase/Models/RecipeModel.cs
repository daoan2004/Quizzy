using Microsoft.EntityFrameworkCore;
using ProjectBase.Controllers;
using ProjectBase.Models.DAO;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ProjectBase.Models
{
    [Table("Recipe")]
    public class RecipeModel
    {
        [Key]
        public long ID { get; set; } // Khóa chính của Recipe
        public long PricePackage_ID { get; set; } // Khóa ngoại liên kết đến PricePackage
        public long UserID { get; set; } // Khóa ngoại liên kết đến User
        public long SubjectID { get; set; } // Khóa ngoại liên kết đến Subject
        public long PricePackage_Type { get; set; }
        public DateTime BuyAt { get; set; } // Thời gian mua
        public DateTime EndAt { get; set; } // Thời gian kết thúc
        public string Status { get; set; } // Trạng thái của Recipe, ví dụ: "Pending", "Paid", "Completed", "Cancelled", v.v.
        

        [ForeignKey("PricePackage_ID")]
        [JsonIgnore]
        public PricePackageModel PricePackage { get; set; } // Navigation property đến PricePackage

        [JsonIgnore]
        [ForeignKey("UserID")]
        public User User { get; set; } // Navigation property đến Use
        [ForeignKey("SubjectID")]
        public SubjectsModel Subjects { get; set; }
    }
}
