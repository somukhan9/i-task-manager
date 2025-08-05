namespace Application.Models;

public class BackgroundServiceTracking
{
    public override bool Equals(object? obj)
    {
        if (obj != null && obj is BackgroundServiceTracking other)
        {
            return TaskId == other.TaskId;
        }
        return false;
    }
    public override int GetHashCode()
    {
        // Generate a hash code based on TaskId
        return TaskId.GetHashCode();
    }
    public string TaskId { get; set; } = default!;
    public bool IsRunnig { get; set; } = true;
    public bool IsInInterval { get; set; } = false;
    public string Name { get; set; } = default!;
    public string? Message { get; set; } = string.Empty;
}