using System.ComponentModel.DataAnnotations;

namespace ProjectBase.Models;

public class LoginModel
{
    [Required]
    [EmailAddress]
    public string email { get; set; } = string.Empty;

    [Required]
    public string password { get; set; } = string.Empty;
}
