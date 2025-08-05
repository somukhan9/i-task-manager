using TaskEntity = Domain.Entities.Task;

namespace Application.Contracts;

public interface ITaskService
{
    Task<IEnumerable<TaskEntity>> GetTasksAsync(int userId, string? query, CancellationToken cancellationToken);
    Task<TaskEntity?> GetTaskByIdAsync(int id, CancellationToken cancellationToken);
    Task<int> AddTaskAsync(TaskEntity task, CancellationToken cancellationToken);
    Task<int> UpdateTaskAsync(TaskEntity task, CancellationToken cancellationToken);
    Task<bool> DeleteTaskAsync(int taskId, CancellationToken cancellationToken);
    Task<bool> DeleteFinishedTask(CancellationToken cancellationToken);

    Task<bool> UpgradeTaskPriority(int taskId, CancellationToken cancellationToken);
    Task<bool> DowngradeTaskPriority(int taskId, CancellationToken cancellationToken);
}