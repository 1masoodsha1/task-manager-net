using System.ComponentModel.DataAnnotations;

namespace TaskManagerBackend.Models;

public class TaskItem : IValidatableObject
{
    [Key]
    public long Id { get; set; }

    [Required]
    [MinLength(1)]
    [MaxLength(120)]
    public string Title { get; set; } = null!;

    [MaxLength(1000)]
    public string? Description { get; set; }

    [Required]
    public TaskStatus Status { get; set; } = TaskStatus.TODO;

    public DateTime? DueDate { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (string.IsNullOrWhiteSpace(Title))
        {
            yield return new ValidationResult(
                "Title is required.",
                new[] { nameof(Title) });
        }

        if (DueDate.HasValue && DueDate.Value < DateTime.UtcNow && Status != TaskStatus.DONE)
        {
            yield return new ValidationResult(
                "Past due dates are only allowed for completed tasks.",
                new[] { nameof(DueDate), nameof(Status) });
        }
    }
}