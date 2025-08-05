using Infrastructure;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Web.ViewModels;

public class TaskViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = default!;
    [DataType(DataType.MultilineText)] public string? Description { get; set; } = default!;
    public int? Priority { get; set; }
    public string? Creator { get; set; } = default!;
    [RegularExpression("^(1|0|-1)", ErrorMessage = "Invalid Status")] // 1 => Finished; 0 => Unfinished; -1 => In Progress
    public string? Status { get; set; } = default!;
    [Display(Name = "TaskDomain Status")]
    public bool IsFinished { get; set; }
    public List<SelectListItem> StatusList = new List<SelectListItem>()
    {
        new SelectListItem() {Value = "1", Text =  Sd.Finished},
        new SelectListItem() {Value = "0", Text = Sd.Unfinished},
        new SelectListItem() {Value = "-1", Text = Sd.InProgress}
    };

    //[BindNever]
    public Dictionary<string, string> StatusMap = new Dictionary<string, string>()
    {
        { "1", Sd.Finished },
        { "0", Sd.Unfinished },
        { "-1", Sd.InProgress },
    };
}