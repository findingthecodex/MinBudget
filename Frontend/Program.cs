using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MinBudget;
using MinBudget.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var apiBaseAddress = builder.HostEnvironment.IsDevelopment()
    ? "https://localhost:7082"
    : builder.HostEnvironment.BaseAddress;

// HttpClient for API
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(apiBaseAddress) });

// LocalStorage & Token Management
builder.Services.AddSingleton<MinBudget.Service.LocalStorageService>();
builder.Services.AddScoped<AuthTokenService>();
builder.Services.AddScoped<ApiClientService>();

// Domain Services
builder.Services.AddScoped<MinBudget.Domain.BudgetManager>();

await builder.Build().RunAsync();