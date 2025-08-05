using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class Task
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Task title is required")]
    public string Title { get; set; } = default!;

    public string? Description { get; set; } = default!;
    public string AttachmentUrl { get; set; } = default!;
    public int Finished { get; set; } = 0; // -1 => WIP; 0 => Undone; 1 => Done
    public DateTime? FinishedDate { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime? UpdatedDate { get; set; }

    public int Priority { get; set; }

    public int UserId { get; set; }
    public User? User { get; set; }
}