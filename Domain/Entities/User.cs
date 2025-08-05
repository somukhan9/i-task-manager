using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Domain.Entities;

public class User : IdentityUser<int>
{
    [Required(ErrorMessage = "Please enter your Name")]
    public string Name { get; set; } = default!;

    [Required(ErrorMessage = "Please enter your Username")]
    public override string? UserName { get; set; }

    [Required] public override string? NormalizedUserName { get; set; }

    [Required(ErrorMessage = "Please enter your Email")]
    public override string? Email { get; set; }

    [Required] public override string? NormalizedEmail { get; set; }

    public string? ProfileImageUrl { get; set; } = default!;

    public DateTime? CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }
}