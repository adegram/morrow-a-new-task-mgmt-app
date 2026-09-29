namespace Taskflow.Models;

public sealed class TaskItem
{
    public long Id { get; set; }
    public string Title { get; set; } = "";
    public string? Notes { get; set; }
    public string Category { get; set; } = "Personal";
    public string Priority { get; set; } = "Medium";
    public DateTime? DueDate { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime CreatedAt { get; set; }
}
