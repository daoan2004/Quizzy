using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using ProjectBase.Models.DAO;

namespace ProjectBase.Models
{

    [Table("Slider")]
    [PrimaryKey("ID")]
    public class SliderModel
    {
        
        public long ID { get; set; }
        public long userID { get; set; }
        public string Title { get; set; } = null!;
        public string image { get; set; } = null!;
        public string backlink { get; set; } = null!;
        public string description { get; set; } = null!;
        public bool status { get; set; }
        public DateTime publishAt { get; set; }
        public DateTime updatedAt { get; set; }

        [ForeignKey("userID")]
        public User User { get; set; } = null!;
    }
}
