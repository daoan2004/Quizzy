using Microsoft.EntityFrameworkCore;
using ProjectBase.Models.DAO;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace ProjectBase.Models.DAO
{
    [Table("Users")]
    [PrimaryKey("ID")]
    public class User
    {

        public long? ID { get; set; }
        public string email { get; set; }
        public string fullname { get; set; }
        public string password { get; set; }
        public string? address { get; set; }
        public string Phone { get; set; }
        public bool gender { get; set; }
        public DateOnly? Dob { get; set; }
        public long? RoleID { get; set; }
        public string? profile_picture { get; set; }
        public DateTime? register_date { get; set; }
        public string? description { get; set; }
        public int? status { get; set; }
        public string? verificationToken {  get; set; }
        public string? PasswordResetToken { get; set; } 
        public DateTime? PasswordResetTokenExpires { get; set; }
        public Role? Role { get; set; }
        public ICollection<PracticeModel> Practice { get; set; }

        public ICollection<RecipeModel> Recipes { get; set; }

        public ICollection<SliderModel> Sliders { get; set; }
    }
}
    public class Role
    {
        public long RoleID { get; set; }
        public string RoleName { get; set; }
        public ICollection<User> Users { get; set; }
    }

        


