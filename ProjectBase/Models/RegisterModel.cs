using System.ComponentModel.DataAnnotations;

namespace ProjectBase.Models;

public class RegisterModel
{
    [Required]
    [StringLength(100)]
    public string fullname { get; set; } = string.Empty;

    [Required]
    public string password { get; set; } = string.Empty;

    [Required]
    [Compare(nameof(password), ErrorMessage = "Password and Confirm Password do not match")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string email { get; set; } = string.Empty;

    [Required]
    [RegularExpression(@"^(\+?\d{1,3}[- ]?)?\d{10}$")]
    public string Phone { get; set; } = string.Empty;

    public bool gender { get; set; }
    public long RoleID { get; set; }
    public int status { get; set; }
    public string? verificationToken { get; set; }
}
