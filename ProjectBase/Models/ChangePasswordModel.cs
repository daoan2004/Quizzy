using System.ComponentModel.DataAnnotations;

namespace ProjectBase.Models;

public class ChangePasswordModel
{
    [Required]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required]
    public string NewPassword { get; set; } = string.Empty;

    [Required]
    [Compare(nameof(NewPassword), ErrorMessage = "New password and confirmation do not match")]
    public string ConfirmNewPassword { get; set; } = string.Empty;
}
