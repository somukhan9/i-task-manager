using Application.Contracts;
using Domain.Entities;
using Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Web.ViewModels;

namespace Web.Controllers;

public class AuthController(IAuthService authService, RoleManager<Role> roleManager, ILogger<AuthController> logger)
    : Controller
{
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return View(model);

        try
        {
            var token =
                await authService.LoginAsync(model.Username, model.Password, model.RememberMe, cancellationToken);

            HttpContext.Session.SetString("JwtToken", token);

            return RedirectToAction("Index", "Home");
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            logger.LogError($"ERROR MESSAGE :: {ex.Message}");
            logger.LogError($"ERROR :: {ex}");
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }
    }

    public async Task<IActionResult> Register()
    {
        if (!await roleManager.RoleExistsAsync(Sd.Admin).ConfigureAwait(false))
        {
            await roleManager.CreateAsync(new Role() { Name = Sd.Admin }).ConfigureAwait(false);
            await roleManager.CreateAsync(new Role() { Name = Sd.User }).ConfigureAwait(false);
        }

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return View(model);

        try
        {
            await authService.RegisterUserAsync(model.Name, model.Username, model.Email, model.Password, model.Role,
                cancellationToken);
            return RedirectToAction(nameof(Login));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            logger.LogError($"ERROR MESSAGE :: {ex.Message}");
            logger.LogError($"ERROR :: {ex}");
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }
    }

    [HttpPost]
    public IActionResult Logout(CancellationToken cancellationToken)
    {
        authService.SignOutAsync(cancellationToken);
        HttpContext.Session.Remove("JwtToken");
        return RedirectToAction("Login");
    }
}