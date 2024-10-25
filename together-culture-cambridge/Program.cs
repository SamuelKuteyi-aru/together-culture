using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using together_culture_cambridge.Data;
using together_culture_cambridge.Models;

var builder = WebApplication.CreateBuilder(args);


var connectionString = builder.Configuration.GetConnectionString("ApplicationDatabaseContext") ??
                       throw new InvalidOperationException("Connection string 'ApplicationDatabaseContext' not found.");
builder.Services.AddDbContextPool<ApplicationDatabaseContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
 
// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();


using (var serviceScope = app.Services.CreateScope())
{
    var services = serviceScope.ServiceProvider;
    MembershipSeed.Initialize(services);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();