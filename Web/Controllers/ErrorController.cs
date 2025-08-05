using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Web.ViewModels;

namespace Web.Controllers;

public class ErrorController : Controller
{
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Index(string? message)
    {

        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier, Message = string.IsNullOrEmpty(message) ? "Some went wrong. Please contact administrator." : message });
    }


    public IActionResult NotFound(string? message)
    {
        message = string.IsNullOrEmpty(message) ? "Requested Resource was not found." : message;
        return View(message);
    }

    [HttpGet("[controller]/access-denied")]
    public IActionResult AccessDenied()
    {
        return View();
    }
}
