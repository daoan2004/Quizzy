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
        public string GroupID { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string QA { get; set; } = null!;
        public string QB { get; set; } = null!;
        public string QC { get; set; } = null!;
        public string QD { get; set; } = null!;
        public string QE { get; set; } = null!;
        public string QF { get; set; } = null!;
        public string Qcorrect { get; set; } = null!;

        [JsonIgnore]
        public ICollection<QuizHandleModel> QuizHandle { get; set; } = [];

    }
}
