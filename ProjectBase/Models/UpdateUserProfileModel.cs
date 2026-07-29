using System.ComponentModel.DataAnnotations;

namespace ProjectBase.Models;

public class UpdateUserProfileModel
{
    [Required]
    [StringLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Required]
    [RegularExpression(@"^(\+?\d{1,3}[- ]?)?\d{10}$")]
    public string Phone { get; set; } = string.Empty;

    public bool Gender { get; set; }
    public string? Address { get; set; }
    public DateOnly? DOB { get; set; }
    public string? Description { get; set; }
    public IFormFile? Avatar { get; set; }
}
