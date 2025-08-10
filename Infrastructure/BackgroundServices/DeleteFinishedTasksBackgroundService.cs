
using Application.Contracts;
using Infrastructure.Common;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Infrastructure.BackgroundServices;

public class DeleteFinishedTasksBackgroundService : AppBackgroundService
{
    private int secondToInterval = 10;

    public override string Name => "DeleteFinishedTasksBackgroundService";

    private readonly IServiceScopeFactory _scopeFactory;

    public DeleteFinishedTasksBackgroundService(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public override async Task AppExecuteAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine($"BACKGROUND SERVICE IS ON :::::::: {DateTime.Now}");

        using var scope = _scopeFactory.CreateScope();

        var taskService = scope.ServiceProvider.GetService<ITaskService>();
        var logger = scope.ServiceProvider.GetService<ILogger<DeleteFinishedTasksBackgroundService>>();

        while (!stoppingToken.IsCancellationRequested)
        {
            var now = DateTime.Now.TimeOfDay;

            try
            {
                if (IsRunning)
                {
                    // Check if current time matches any run time (within 1 minute tolerance)
                    var result = await taskService!.DeleteFinishedTask(stoppingToken).ConfigureAwait(false);
                    SetBackgroundStatus(result);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                logger!.LogError($"ERROR MESSAGE :: {ex.Message}");
                logger!.LogError($"ERROR :: {ex}");
            }

            //Console.WriteLine($"Task.Delay ::::: {secondToInterval}");
            await Task.Delay(TimeSpan.FromSeconds(secondToInterval), stoppingToken).ConfigureAwait(false);
        }
    }

    public void SetBackgroundStatus(bool running)
    {
        var result = running ? "Running" : "Interval";

        if (!running/* && !this.IsInterval*/)
        {
            secondToInterval = 10; // Poll every 10 seconds until the time matches
            IsInterval = false;
            Message = $"Background reactivated :: {result}";

            Console.WriteLine($"Reactivation :: IsInterval => {IsInterval} ::::: secondToInterval => {secondToInterval}");
        }
        else if (running/* && this.IsInterval*/)
        {

            secondToInterval = 60; // Wait 1 minute to avoid duplicate execution within the same minute
            IsInterval = true;
            Message = $"Background went to cooldown after :: {result}";

            Console.WriteLine($"Cool Down :: IsInterval => {IsInterval} ::::: secondToInterval => {secondToInterval}");
        }

        Console.WriteLine($"Message :: {Message}");
    }

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        IsRunning = false;
        return base.StopAsync(cancellationToken);
    }
}
