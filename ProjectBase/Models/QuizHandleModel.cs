using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace ProjectBase.Models
{
    [Table("QuizHandle")]
    [PrimaryKey("ID")]
    public class QuizHandleModel
    {
        public long ID { get; set; }  
        public long UserID { get; set; }
        public long PracticeID { get; set; }
        public long QuizID { get; set; }
        public string QAnswer { get; set; } 

        public bool isMark {  get; set; } 
        public bool status { get; set; } 
        public bool isCorrect { get; set; }
        
        [ForeignKey("QuizID")]
        public QuizBankModel QuizBank { get; set; }

        [NotMapped]
        public string StatusDescription
        {
            get
            {
                return status ? "Answered" : "Not yet answered";
            }
        }
    }
}
