using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ProjectBase.Models
{
    [Table("PracticeLevel")]
    [PrimaryKey("ID")]
    public class PracticeLevel
    {
        public int ID { get; set; }
        public string title { get; set; } = null!;
        public string Description { get; set; } = null!;
        [JsonIgnore]
        public ICollection<PracticeModel> Practice { get; set; } = [];
        [JsonIgnore]
        public ICollection<SimulationExam> Exams { get; set; } = [];

    }
}
