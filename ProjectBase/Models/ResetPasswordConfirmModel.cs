using System.ComponentModel.DataAnnotations;

namespace ProjectBase.Models;

public class ResetPasswordConfirmModel
{
    [Required]
    public string NewPassword { get; set; } = string.Empty;

    [Required]
    [Compare(nameof(NewPassword), ErrorMessage = "Passwords do not match")]
    public string ReNewPassword { get; set; } = string.Empty;

    [Required]
    public string Token { get; set; } = string.Empty;
}
