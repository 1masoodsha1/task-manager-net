using Microsoft.EntityFrameworkCore;
using TaskManagerBackend.Models;

namespace TaskManagerBackend.Data;

public class TaskContext : DbContext
{
    public TaskContext(DbContextOptions<TaskContext> options) : base(options) { }

    public DbSet<TaskItem> Tasks => Set<TaskItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // store enum as string (so JSON / DB values are "TODO", "IN_PROGRESS", "DONE")
        modelBuilder.Entity<TaskItem>()
            .Property(t => t.Status)
            .HasConversion<string>();
    }
}
