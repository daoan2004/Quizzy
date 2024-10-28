using System.ComponentModel.DataAnnotations;

namespace ProjectBase.Models
{
    public class ResetPasswordConfirmModel
    {
        [Required]
        public string NewPassword { get; set; }

        [Required]
        public string ReNewPassword { get; set; }

        [Required]
        public string Token { get; set; }
    }

}
