using System.ComponentModel.DataAnnotations;

namespace TaskManagerBackend.Models;

public class TaskItem
{
    [Key]
    public long Id { get; set; }

    [Required]
    [MaxLength(255)]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Required]
    public TaskStatus Status { get; set; } = TaskStatus.TODO;

    public DateTime? DueDate { get; set; }
}
