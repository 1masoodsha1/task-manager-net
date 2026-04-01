using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using TaskManagerBackend.Data;
using TaskManagerBackend.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

var dbPath = Path.Combine(builder.Environment.ContentRootPath, "appdb.db");

builder.Services.AddDbContext<TaskContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalFrontend", policy =>
        policy.WithOrigins(
                "http://localhost:4200",
                "http://127.0.0.1:4200",
                "http://localhost:5173",
                "http://127.0.0.1:5173")
              .AllowAnyHeader()
              .AllowAnyMethod());
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("AllowLocalFrontend");

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TaskContext>();
    db.Database.EnsureCreated();
    DbSeeder.Initialize(db);
}

app.MapGet("/", () => Results.Ok(new
{
    name = "Task Manager API",
    version = "v1",
    framework = ".NET 10"
}));

var tasks = app.MapGroup("/api/tasks").WithTags("Tasks");

tasks.MapGet("/", async (TaskContext db) =>
{
    var items = await db.Tasks
        .OrderBy(t => t.Id)
        .ToListAsync();

    return Results.Ok(items);
});

tasks.MapGet("/{id:long}/", async (long id, TaskContext db) =>
{
    var task = await db.Tasks.FindAsync(id);
    return task is null
        ? Results.NotFound(new { detail = "Not found" })
        : Results.Ok(task);
});

tasks.MapPost("/", async (TaskItem input, TaskContext db) =>
{
    var validationResult = ValidateTask(input);
    if (validationResult is not null)
    {
        return validationResult;
    }

    db.Tasks.Add(input);
    await db.SaveChangesAsync();

    return Results.Created($"/api/tasks/{input.Id}/", input);
});

tasks.MapPut("/{id:long}/", async (long id, TaskItem updated, TaskContext db) =>
{
    var validationResult = ValidateTask(updated);
    if (validationResult is not null)
    {
        return validationResult;
    }

    var existing = await db.Tasks.FindAsync(id);

    if (existing is null)
    {
        return Results.NotFound(new { detail = "Not found" });
    }

    existing.Title = updated.Title;
    existing.Description = updated.Description;
    existing.Status = updated.Status;
    existing.DueDate = updated.DueDate;

    await db.SaveChangesAsync();
    return Results.Ok(existing);
});

tasks.MapDelete("/{id:long}/", async (long id, TaskContext db) =>
{
    var existing = await db.Tasks.FindAsync(id);

    if (existing is null)
    {
        return Results.NotFound(new { detail = "Not found" });
    }

    db.Tasks.Remove(existing);
    await db.SaveChangesAsync();

    return Results.NoContent();
});

app.Run();

static IResult? ValidateTask(TaskItem task)
{
    task.Title = (task.Title ?? string.Empty).Trim();
    task.Description = string.IsNullOrWhiteSpace(task.Description)
        ? null
        : task.Description.Trim();

    if (string.IsNullOrWhiteSpace(task.Title))
    {
        return Results.BadRequest(new { detail = "Title is required" });
    }

    if (task.Title.Length > 255)
    {
        return Results.BadRequest(new { detail = "Title must be 255 characters or fewer" });
    }

    if (task.DueDate.HasValue)
    {
        var dueDateUtc = task.DueDate.Value.Kind switch
        {
            DateTimeKind.Utc => task.DueDate.Value,
            DateTimeKind.Local => task.DueDate.Value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(task.DueDate.Value, DateTimeKind.Utc)
        };

        task.DueDate = dueDateUtc;

        if (task.Status != TaskManagerBackend.Models.TaskStatus.DONE && dueDateUtc < DateTime.UtcNow)
        {
            return Results.BadRequest(new
            {
                dueDate = "Past due date/time is only allowed for completed tasks."
            });
        }
    }

    return null;
}