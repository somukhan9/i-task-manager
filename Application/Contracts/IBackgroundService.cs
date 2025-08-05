using Domain.Entities;

namespace Application.Contracts;

public interface IBackgroundService
{
    Task<List<BackgroundServices>> ListOfAvailableServices();
}
