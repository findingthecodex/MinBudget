using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MinBudget;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddSingleton<MinBudget.Service.LocalStorageService>(); // Ändring: registrerar LocalStorageService
builder.Services.AddScoped<MinBudget.Domain.BudgetManager>(); // Ändring: registrerar BudgetManager som scoped service

await builder.Build().RunAsync();