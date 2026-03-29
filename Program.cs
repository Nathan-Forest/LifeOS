using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using LifeOS.Data;

var builder = WebApplication.CreateBuilder(args);

// ============================
// THE FRIDGE - Database setup
// ============================
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// ============================
// THE LOCKED DOOR - JWT Auth
// ============================
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!))
        };
    });

builder.Services.AddAuthorization();

// ============================
// THE PASS WINDOW - CORS
// ============================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
                "http://localhost:5173",      // Vite dev server
                "http://localhost:3000",      // CloudControl
                "http://192.168.50.160:3000" // CloudControl on server
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// ============================
// SWAGGER - API documentation
// ============================
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = 
            System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ============================
// AUTO-MIGRATE on startup
// ============================
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

// ============================
// MIDDLEWARE PIPELINE
// ============================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// ============================
// HEALTH CHECK - for PulseMonitor
// ============================
app.MapGet("/health", () => Results.Ok(new
{
    service = "LifeOS API",
    status = "healthy",
    timestamp = DateTime.UtcNow
}));

// ============================
// STATS - for CloudControl homepage
// ============================
app.MapGet("/stats", async (AppDbContext db) =>
{
    var today = DateTime.UtcNow.Date;

    return Results.Ok(new
    {
        habitsToday = await db.HabitCompletions
            .CountAsync(hc => hc.CompletedAt.Date == today),
        activeHabits = await db.Habits
            .CountAsync(h => h.IsActive),
        activeGoals = await db.Goals
            .CountAsync(g => g.Status == "active"),
        openTasks = await db.ProjectTasks
            .CountAsync(t => !t.IsComplete),
        activeProjects = await db.Projects
            .CountAsync(p => p.Status == "active"),
        studyMinutesToday = await db.StudySessions
            .Where(s => s.StartTime.Date == today)
            .ToListAsync() // Pull into memory first
            .ContinueWith(t => (int)t.Result
            .Sum(s => (s.EndTime - s.StartTime).TotalMinutes)),
        timestamp = DateTime.UtcNow
    });
});

app.Run();