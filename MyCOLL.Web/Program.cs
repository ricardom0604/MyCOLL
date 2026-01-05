using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MyCOLL.Web;
using MyCOLL.Shared.Services;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using MyCOLL.Shared.Interface;
using MyCOLL.Web.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// 1. HttpClient
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("https://localhost:7004") });

// 2. Auth e Storage
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddAuthorizationCore();

builder.Services.AddScoped<ClientAuthStateProvider>(); // Regista a classe concreta
builder.Services.AddScoped<AuthenticationStateProvider>(p => p.GetRequiredService<ClientAuthStateProvider>());
builder.Services.AddScoped<IAuthService>(p => p.GetRequiredService<ClientAuthStateProvider>());
// 3. Seus Serviços
builder.Services.AddSingleton<CartService>();
await builder.Build().RunAsync();