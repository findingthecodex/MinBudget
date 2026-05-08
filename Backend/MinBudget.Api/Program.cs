using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MinBudget.Application.Services;
using MinBudget.Domain.Interfaces;
using MinBudget.Infrastructure.Data;
using MinBudget.Infrastructure.Repositories;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add Database
// Azure App Service sets HOME to persistent storage: /home (Linux) or D:\home (Windows)
var homeDir = Environment.GetEnvironmentVariable("HOME");
var dbPath = homeDir != null
    ? Path.Combine(homeDir, "minbudget.db")
    : "minbudget.db";
Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(dbPath))!);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite($"Data Source={dbPath}"));

// Add Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

// Configure Password
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
});

// JWT Configuration
var jwtKey = builder.Configuration["Jwt:Key"] ?? "your-super-secret-key-change-in-production-at-least-32-chars-long";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "MinBudget";
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? "MinBudgetClient";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

// CORS Configuration (for Blazor WASM)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazor", policy =>
    {
        policy.WithOrigins("https://localhost:7191", "https://localhost:7001", "http://localhost:5069", "http://localhost:5001", "http://localhost:5173")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Register Repositories
builder.Services.AddScoped<IExpenseRepository, ExpenseRepository>();
builder.Services.AddScoped<IIncomeRepository, IncomeRepository>();

// Register Services
builder.Services.AddScoped<IExpenseService, ExpenseService>();
builder.Services.AddScoped<IIncomeService, IncomeService>();
builder.Services.AddScoped<AuthService<ApplicationUser>>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Auto-migrate database
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Prevent browsers from caching CSS and the Blazor boot manifest between deployments.
// MinBudget.styles.css has no content hash in its name so stale caches cause scoped
// CSS attributes (b-xxxx) in the HTML to stop matching the rules in the CSS file.
app.Use(async (context, next) =>
{
    var path = context.Request.Path.Value ?? "";
    if (path.EndsWith(".css", StringComparison.OrdinalIgnoreCase) ||
        path.EndsWith("blazor.boot.json", StringComparison.OrdinalIgnoreCase))
    {
        context.Response.OnStarting(() =>
        {
            context.Response.Headers["Cache-Control"] = "no-cache";
            return Task.CompletedTask;
        });
    }
    await next();
});

app.UseBlazorFrameworkFiles();
app.UseStaticFiles();
app.UseCors("AllowBlazor");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapFallbackToFile("index.html");

app.Run();

