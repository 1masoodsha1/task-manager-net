using TaskManagerBackend.Models;

namespace TaskManagerBackend.Data;

public static class DbSeeder
{
    public static void Initialize(TaskContext context)
    {
        if (context.Tasks.Any())
        {
            return;
        }

        var now = DateTime.UtcNow;

        context.Tasks.AddRange(
            new TaskItem
            {
                Title = "Buy groceries",
                Description = "Milk, eggs, bread, fruit, and rice.",
                Status = TaskManagerBackend.Models.TaskStatus.TODO,
                DueDate = now.AddDays(1)
            },
            new TaskItem
            {
                Title = "Do laundry",
                Description = "Wash clothes and fold them in the evening.",
                Status = TaskManagerBackend.Models.TaskStatus.IN_PROGRESS,
                DueDate = now.AddHours(8)
            },
            new TaskItem
            {
                Title = "Clean the room",
                Description = "Vacuum the floor and organize the desk.",
                Status = TaskManagerBackend.Models.TaskStatus.TODO,
                DueDate = now.AddDays(2)
            },
            new TaskItem
            {
                Title = "Pay electricity bill",
                Description = "Check the online account and make the payment.",
                Status = TaskManagerBackend.Models.TaskStatus.TODO,
                DueDate = now.AddDays(3)
            }
        );

        context.SaveChanges();
    }
}