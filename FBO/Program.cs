using FBO.Dapper;
using FBO.Services;
using Serilog;

var path = Directory.GetCurrentDirectory();
var builder = WebApplication.CreateBuilder(args);
Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File($"{path}\\Logs\\Log.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

// Add services to the container.
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
builder.Services.AddScoped<Dapperr, Dapperr>();
builder.Services.AddScoped<UtilitiesService, UtilitiesService>();
builder.Services.AddScoped<FBOMainService, FBOMainService>();
builder.Services.AddScoped<GeneralService, GeneralService>();
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

app.Run();








