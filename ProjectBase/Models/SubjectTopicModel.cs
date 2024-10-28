using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace ProjectBase.Models
{
    [Table("SubjectTopic")]
    [PrimaryKey("id")]
    public class SubjectTopicModel
    {
       
        public int id {  get; set; }
        public long subjectId { get; set; }
        public string title { get; set; }

        [JsonIgnore]
        public ICollection<PracticeModel> Practice { get; set; }
    }
}
