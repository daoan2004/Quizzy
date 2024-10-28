using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ProjectBase.Models
{
    [Table("QuizBank")]
    [PrimaryKey("ID")]
    public class QuizBankModel
    {
        public long ID { get; set; }
        public long SubjectID { get; set; }
        public int TopicID { get; set; }
        public int LevelID { get; set; }
        public bool Status { get; set; }
        public string GroupID { get; set; }
        public string Title { get; set; }
        public string QA { get; set; }
        public string QB { get; set; }
        public string QC { get; set; }
        public string QD { get; set; }
        public string QE { get; set; }
        public string QF { get; set; }
        public string Qcorrect { get; set; }

        [JsonIgnore]
        public ICollection<QuizHandleModel> QuizHandle { get; set; }

    }
}
