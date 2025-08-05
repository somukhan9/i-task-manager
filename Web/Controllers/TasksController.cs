using Application.Contracts;
using Domain.Entities;
using Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Web.ViewModels;
using TaskDomain = Domain.Entities.Task;

namespace Web.Controllers;

[Authorize]
public class TasksController(UserManager<User> userManager, ITaskService taskService, ILogger<TasksController> logger)
    : Controller
{
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var userId = userManager.GetUserId(User);
        var tasks = await taskService.GetTasksAsync(int.Parse(userId!), string.Empty, cancellationToken);

        var model = tasks.Select(t => new TaskViewModel()
        {
            Id = t.Id,
            Title = t.Title,
            Creator = t.User!.Name,
            Priority = t.Priority,
            Status = t.Finished.ToString()
        });

        return View(model);
    }

    //[TaskOwnerAuthorize(id: "taskId")]
    [Authorize(Policy = Sd.TaskOwnerPolicy)]
    public async Task<IActionResult> Upsert(int? taskId, CancellationToken cancellationToken)
    {
        var model = new TaskViewModel();

        if (taskId is null || taskId == 0)
        {
            return View(model);
        }

        var task = await taskService.GetTaskByIdAsync(taskId.Value, cancellationToken);

        if (task is null)
        {
            return NotFound();
        }

        var userId = userManager.GetUserId(User);

        model.Id = task.Id;
        model.Title = task.Title;
        model.Description = task.Description;
        model.Status = task.Finished.ToString();

        return View(model);
    }

    [HttpPost]
    //[TaskOwnerAuthorize(id: "taskId")]
    [Authorize(Policy = Sd.TaskOwnerPolicy)]
    public async Task<IActionResult> Upsert(TaskViewModel model, CancellationToken cancellationToken)
    {
        try
        {
            var userId = int.Parse(userManager.GetUserId(User)!);

            if (model.Id == 0)
            {
                // Create
                if (!ModelState.IsValid)
                {
                    foreach (var error in ModelState.Values.SelectMany(modelState => modelState.Errors))
                    {
                        ModelState.AddModelError(string.Empty, error.ErrorMessage);
                    }

                    return View(model);
                }


                var task = new TaskDomain()
                {
                    Title = model.Title,
                    Description = model.Description,
                    UserId = userId,
                    AttachmentUrl = string.Empty
                };
                var taskId = await taskService.AddTaskAsync(task, cancellationToken);

                if (taskId == 0)
                {
                    ModelState.AddModelError(string.Empty, "Error occured while creating the task.");
                }

                return RedirectToAction(nameof(Index));
            }
            else
            {
                // Update
                var task = await taskService.GetTaskByIdAsync(model.Id, cancellationToken);

                if (task is null)
                {
                    return NotFound();
                }

                if (!ModelState.IsValid)
                {
                    foreach (var error in ModelState.Values.SelectMany(modelState => modelState.Errors))
                    {
                        ModelState.AddModelError(string.Empty, error.ErrorMessage);
                    }

                    return View(model);
                }

                task.Title = model.Title ?? task.Title;
                task.Description = model.Description ?? task.Description;
                task.Finished = int.Parse(model.Status!);

                await taskService.UpdateTaskAsync(task, cancellationToken);

                return RedirectToAction(nameof(Index));
            }
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
}