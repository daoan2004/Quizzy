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
        public string Title { get; set; }
        public string image { get; set; }   
        public string backlink { get; set; }
        public string description { get; set; }
        public bool status { get; set; }
        public DateTime publishAt { get; set; }
        public DateTime updatedAt { get; set; }

        [ForeignKey("UserID")]
        public User User { get; set; }
    }
}
