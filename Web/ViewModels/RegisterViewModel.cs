using System.ComponentModel.DataAnnotations;
using Infrastructure;

namespace Web.ViewModels;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Please provide a name for the user.")]
    public string Name { get; init; } = default!;

    [Required(ErrorMessage = "Please provide a unique username for the user.")]
    public string Username { get; init; } = default!;

    [Required(ErrorMessage = "Please provide a unique email for the user.")]
    [EmailAddress(ErrorMessage = "Please provide a valid email address for the user.")]
    public string Email { get; init; } = default!;

    [Required(ErrorMessage = "Please provide a valid password for the user.")]
    [DataType(DataType.Password)]
    public string Password { get; init; } = default!;

    [Compare("Password", ErrorMessage = "Passwords do not match.")]
    [Display(Name = "Confirm Password")]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; init; } = default!;

    public string Role { get; init; } = Sd.User;
}