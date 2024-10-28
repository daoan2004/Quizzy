using Microsoft.EntityFrameworkCore;
using ProjectBase.Models.DAO;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ProjectBase.Models
{
    [Table("Practice")]
    [PrimaryKey("ID")]
    public class PracticeModel
    {
        public long ID { get; set; }
        public long UserID { get; set; }
        public long SubjectID { get; set; }
        public string title { get; set; }
        public DateTime taken_date { get; set; }
        public TimeOnly duration { get; set; }
        public int number_quest { get; set; }
        public int number_correct { get; set; }
        public int levelID { get; set; }

        public int topicID { get; set; }
        public TimeOnly time_taken { get; set; }

        public string Quest_group { get; set; }
        public bool Status { get; set; }
        [NotMapped]
        [ForeignKey("UserID")]
        public User User { get; set; }
        [NotMapped]
        [ForeignKey("SubjectID")]
        public SubjectsModel Subject { get; set; }
        [NotMapped]
        [ForeignKey("levelID")]
        public PracticeLevel Level { get; set; }
        [NotMapped]
        [ForeignKey("topicID")]
        public SubjectTopicModel Topic { get; set; }


    }
}
