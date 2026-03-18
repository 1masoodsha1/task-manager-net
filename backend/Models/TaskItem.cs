using System.ComponentModel.DataAnnotations;

namespace TaskManagerBackend.Models;

public class TaskItem
{
    [Key]
    public long Id { get; set; }

    [Required]
    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    [Required]
    public TaskStatus Status { get; set; } = TaskStatus.TODO;

    // frontend expects ISO date strings; null allowed
    public DateTime? DueDate { get; set; }
}
