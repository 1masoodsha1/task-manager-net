using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using TaskManagerBackend.Data;

var builder = WebApplication.CreateBuilder(args);

// SQLite DB (file)
builder.Services.AddDbContext<TaskContext>(options =>
    options.UseSqlite("Data Source=./data/appdb.db"));

// Controllers + enum as string for JSON
builder.Services.AddControllers()
    .AddJsonOptions(opts =>
    {
        opts.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// Allow Vite (5173) and Angular dev server (4200)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();           // <-- app MUST be declared BEFORE any app.* calls

// Ensure DB + seed sample data
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<TaskContext>();
    db.Database.EnsureCreated();
    DbSeeder.Initialize(db);
}

app.UseCors("AllowFrontend");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
