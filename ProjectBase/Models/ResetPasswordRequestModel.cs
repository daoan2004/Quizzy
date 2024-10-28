using System.ComponentModel.DataAnnotations;

public class ResetPasswordRequestModel
{
    [Required]
    [EmailAddress]
    public string email { get; set; }
}
