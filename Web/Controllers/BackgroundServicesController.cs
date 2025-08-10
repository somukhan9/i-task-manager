using Application.Contracts;
using Infrastructure;
using Infrastructure.BackgroundServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.ViewModels;

namespace Web.Controllers;


[Authorize(Roles = Sd.Admin)]
public class BackgroundServicesController(IBackgroundService backgroundService
    , IBackgroundServicesFactory<DeleteFinishedTasksBackgroundService> deleteFinishedTasksBackgroundService
    , ILogger<BackgroundServicesController> logger) : Controller
{
    public async Task<IActionResult> Index(string? name)
    {
        var model = new BackgroundServicesViewModel()
        {
            ListOfBackgroundServicesName = await backgroundService.ListOfAvailableServices(),
            ListOfRunningServices = deleteFinishedTasksBackgroundService.ListOfRunningServices(name)
        };

        return View(model);
    }

    [HttpPost]
    public async Task<IActionResult> CreateService(string name)
    {
        try
        {
            if (name.ToLower().Trim().Equals("DeleteFinishedTasksBackgroundService".ToLower().Trim()))
            {
                await deleteFinishedTasksBackgroundService.CreateAsync();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            logger.LogError($"ERROR MESSAGE :: {ex.Message}");
            logger.LogError($"ERROR :: {ex}");
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Kill(string taskId, string name)
    {
        try
        {
            if (name.ToLower().Trim().Equals("DeleteFinishedTasksBackgroundService".ToLower().Trim()))
            {
                await deleteFinishedTasksBackgroundService.KillAsync(taskId);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            logger.LogError($"ERROR MESSAGE :: {ex.Message}");
            logger.LogError($"ERROR :: {ex}");
        }

        return RedirectToAction(nameof(Index));
    }
}
