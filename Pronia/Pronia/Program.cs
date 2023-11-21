using Microsoft.EntityFrameworkCore;
using Pronia.DAL;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));
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
