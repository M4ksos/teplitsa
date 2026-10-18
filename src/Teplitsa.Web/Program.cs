using Teplitsa.Web.Components;
using Microsoft.EntityFrameworkCore;
using Teplitsa.Infrastructure;
using Teplitsa.Web.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddInfrastructure("Data Source=teplitsa.db");

builder.Services.AddSingleton<GreenhouseSimulation>();
builder.Services.AddHostedService(sp => sp.GetRequiredService<GreenhouseSimulation>());

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<GreenhouseDbContext>();
    db.Database.Migrate();      // создаёт/обновляет схему при старте
    DbSeeder.Seed(db);

    Console.WriteLine($"Датчиков: {db.Set<Teplitsa.Domain.Sensor>().Count()}, " +
                      $"устройств: {db.Set<Teplitsa.Domain.Actuator>().Count()}");
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
