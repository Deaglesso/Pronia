using Microsoft.EntityFrameworkCore;
using Pronia.DAL;
using Pronia.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));
builder.Services.AddScoped<LayoutService>();
var app = builder.Build();
app.UseStaticFiles();

app.MapControllerRoute(
    "admin",
    "{area:exists}/{controller=dashboard}/{action=index}/{id?}"
);

app.MapControllerRoute(
    "default",
    "{controller=home}/{action=index}/{id?}"
);

app.Run();
