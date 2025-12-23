// keep your model namespace for TaskItem etc.
using TaskManagerBackend.Models;

// alias so TaskStatus refers to your enum
using TaskStatus = TaskManagerBackend.Models.TaskStatus;

namespace TaskManagerBackend.Data;

public static class DbSeeder
{
    public static void Initialize(TaskContext context)
    {
        if (context.Tasks.Any()) return;

        context.Tasks.AddRange(
            new TaskItem
            {
                Title = "Sample task",
                Description = "This is a seeded example task.",
                Status = TaskStatus.TODO,
                DueDate = DateTime.Today.AddDays(7)
            },
            new TaskItem
            {
                Title = "Second task",
                Description = "Another example",
                Status = TaskStatus.IN_PROGRESS
            }
        );

        context.SaveChanges();
    }
}
