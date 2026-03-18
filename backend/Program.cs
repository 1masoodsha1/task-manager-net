using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using TaskManagerBackend.Data;
using TaskManagerBackend.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddDbContext<TaskContext>(options =>
    options.UseInMemoryDatabase("TaskDb"));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalFrontend", policy =>
        policy.WithOrigins("http://localhost:4200")
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
    DbSeeder.Initialize(db);
}

app.MapGet("/", () => Results.Ok(new
{
    name = "Task Manager API",
    version = "v1",
    framework = ".NET 10"
}));

var tasks = app.MapGroup("/api/tasks").WithTags("Tasks");

tasks.MapGet("/", async Task<Ok<IReadOnlyList<TaskItem>>> (TaskContext db) =>
{
    var items = await db.Tasks
        .OrderBy(t => t.DueDate ?? DateTime.MaxValue)
        .ThenBy(t => t.Id)
        .ToListAsync();

    return TypedResults.Ok<IReadOnlyList<TaskItem>>(items);
});

tasks.MapGet("/{id:long}", async Task<Results<Ok<TaskItem>, NotFound>> (long id, TaskContext db) =>
{
    var task = await db.Tasks.FindAsync(id);
    return task is null ? TypedResults.NotFound() : TypedResults.Ok(task);
});

tasks.MapPost("/", async Task<Created<TaskItem>> (TaskItem input, TaskContext db) =>
{
    db.Tasks.Add(input);
    await db.SaveChangesAsync();

    return TypedResults.Created($"/api/tasks/{input.Id}", input);
});

tasks.MapPut("/{id:long}", async Task<Results<Ok<TaskItem>, NotFound>> (long id, TaskItem updated, TaskContext db) =>
{
    var existing = await db.Tasks.FindAsync(id);

    if (existing is null)
    {
        return TypedResults.NotFound();
    }

    existing.Title = updated.Title;
    existing.Description = updated.Description;
    existing.Status = updated.Status;
    existing.DueDate = updated.DueDate;

    await db.SaveChangesAsync();
    return TypedResults.Ok(existing);
});

tasks.MapDelete("/{id:long}", async Task<Results<NoContent, NotFound>> (long id, TaskContext db) =>
{
    var existing = await db.Tasks.FindAsync(id);

    if (existing is null)
    {
        return TypedResults.NotFound();
    }

    db.Tasks.Remove(existing);
    await db.SaveChangesAsync();

    return TypedResults.NoContent();
});

app.Run();