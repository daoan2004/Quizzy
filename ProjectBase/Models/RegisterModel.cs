using Microsoft.EntityFrameworkCore;
using ProjectBase.Models.DAO;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;

namespace ProjectBase.Models
{

    public class RegisterModel
    {        
        public string fullname { get; set; }
        public string password { get; set; }
        public string ConfirmPassword { get; set; }
        public string email { get; set; }
        public string Phone { get; set; }
        public bool gender { get; set; }
        public long RoleID {  get; set; }
        public int status { get; set; }
        public string? verificationToken { get; set; }

        public User toUser()
        {
            return new User
            {
                fullname = this.fullname,
                password = this.password,
                email = this.email,
                Phone = this.Phone,
                gender = this.gender,
                verificationToken = this.verificationToken,
                status = 0,
                RoleID = 2,
            };
        }
       
    }
}
