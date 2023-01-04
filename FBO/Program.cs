using FBO.Dapper;
using FBO.Services;
using FBO.ViewModels;
using Microsoft.Extensions.Options;
using Serilog;
using System.Configuration;

var path = Directory.GetCurrentDirectory();
var builder = WebApplication.CreateBuilder(args);
Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File($"{path}\\Logs\\Log.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

IConfiguration configuration = new ConfigurationBuilder()
                            .AddJsonFile("appsettings.json")
                            .Build();

// Add services to the container.
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
builder.Services.Configure<AppSettings>(configuration).AddSingleton(sp => sp.GetRequiredService<IOptions<AppSettings>>().Value);
builder.Services.AddScoped<Dapperr, Dapperr>();
builder.Services.AddScoped<UtilitiesService, UtilitiesService>();
builder.Services.AddScoped<FBOMainService, FBOMainService>();
builder.Services.AddScoped<GeneralService, GeneralService>();
builder.Services.AddScoped<ServicePhotoService, ServicePhotoService>();
var app = builder.Build();

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
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=FBOHelper}/{action=Index}/{id?}");

app.Run();








