using Application.Models;

namespace Application.Contracts;

public interface IBackgroundServicesFactory<T>
{
    Task<BackgroundServiceTracking> CreateAsync();
    Task<bool> KillAsync(string taskId);
    List<BackgroundServiceTracking> ListOfRunningServices(string? name);
}
