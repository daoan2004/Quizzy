using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ProjectBase.Models {
    [Table("SimulationExam")]
    [PrimaryKey("ID")]
    public class SimulationExam
    {
        public long ID { get; set; }
        public long SubjectID { get; set; }
        public int LevelID { get; set; }
        public string ExamName { get; set; }
        public int Number_Question { get; set; }
        public int Duration { get; set; }
        [Range(0, 100)]
        public decimal Passrate { get; set; } // Tỉ lệ đậu (%)

        [ForeignKey("SubjectID")]
        [JsonIgnore]
        public SubjectsModel Subjects { get; set; }
        [ForeignKey("LevelID")]
        public PracticeLevel Level { get; set; }
    }
}
