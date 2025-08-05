using Application.Contracts;
using Application.Models;
using Infrastructure.Common;
using Microsoft.Extensions.DependencyInjection;


namespace Infrastructure.Services;

class BackgroundServicesFactory<T> : IBackgroundServicesFactory<T> where T : AppBackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;

    public BackgroundServicesFactory(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }

    public async Task<BackgroundServiceTracking> CreateAsync()
    {
        using CancellationTokenSource cancellationTokenSource = new();
        CancellationToken cancellationToken = cancellationTokenSource.Token;
        BackgroundServiceTracking backgroundServiceTracking = new();

        using var scope = _scopeFactory.CreateScope();
        var serviceProvider = scope.ServiceProvider;

        var service = ActivatorUtilities.CreateInstance<T>(serviceProvider);
        backgroundServiceTracking.Name = service.Name;
        backgroundServiceTracking.TaskId = service.TaskId;
        backgroundServiceTracking.IsRunnig = service.IsRunning;
        backgroundServiceTracking.IsInInterval = service.IsInterval;
        backgroundServiceTracking.Message = service.Message;

        await service.StartAsync(cancellationToken);

        BackgroundServicesTrackingStore<T>.BackgroundServices.Add(backgroundServiceTracking, service);

        return backgroundServiceTracking;
    }

    public async Task<bool> KillAsync(string taskId)
    {
        var backgroundServiceTracking = BackgroundServicesTrackingStore<T>.BackgroundServices.Keys.FirstOrDefault(x => x.TaskId == taskId);

        if (backgroundServiceTracking is null)
        {
            return false;
        }

        if (BackgroundServicesTrackingStore<T>.BackgroundServices.ContainsKey(backgroundServiceTracking))
        {
            var service = BackgroundServicesTrackingStore<T>.BackgroundServices[backgroundServiceTracking];
            await service.StopAsync(CancellationToken.None);

            if (!service.IsRunning)
            {
                BackgroundServicesTrackingStore<T>.BackgroundServices.Remove(backgroundServiceTracking);
                return true;
            }
        }

        return false;
    }

    public List<BackgroundServiceTracking> ListOfRunningServices(string? name)
    {
        if (string.IsNullOrEmpty(name))
        {
            return BackgroundServicesTrackingStore<T>.BackgroundServices.Keys.Select(e => new BackgroundServiceTracking()
            {
                Name = e.Name,
                TaskId = e.TaskId,
                IsRunnig = e.IsRunnig,
                IsInInterval = e.IsInInterval,
                Message = e.Message
            }).ToList();
        }

        return BackgroundServicesTrackingStore<T>.BackgroundServices.Keys.Where(b => b.Name.ToLower() == name.ToLower()).Select(e => new BackgroundServiceTracking()
        {
            Name = e.Name,
            TaskId = e.TaskId,
            IsRunnig = e.IsRunnig,
            IsInInterval = e.IsInInterval,
            Message = e.Message
        }).ToList();
    }
}
