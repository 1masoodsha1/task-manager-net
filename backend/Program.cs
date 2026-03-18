using Microsoft.EntityFrameworkCore;
using TaskManagerBackend.Data;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddControllers();

builder.Services.AddDbContext<TaskContext>(options =>
    options.UseInMemoryDatabase("TaskDb"));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy => policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
});

var app = builder.Build();

// Middleware
app.UseCors("AllowAll");

// Root endpoint (so / works)
app.MapGet("/", () => "Task Manager API running 🚀");

// Controllers (IMPORTANT)
app.MapControllers();

app.Run();