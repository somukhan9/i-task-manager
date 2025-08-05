using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "Please provide valid username or email")]
    [Display(Name = "Username or Email")]
    public string Username { get; init; } = default!;

    [Required(ErrorMessage = "Please provide valid password")]
    [DataType(DataType.Password)]
    public string Password { get; init; } = default!;

    public bool RememberMe { get; init; }
}