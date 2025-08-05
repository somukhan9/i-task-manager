namespace Domain.Entities;

public class BackgroundServices
{
    public int Id { get; set; }
    public required string ServiceName { get; set; }
    public int NumberOfApps { get; set; }
}
