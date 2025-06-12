using Microsoft.EntityFrameworkCore;
using Village_Manager.Data;
using Village_Manager.MiddleWare;
using Village_Manager.Repositories;
using Village_Manager.Repositories.IRepositories;
using Village_Manager.Services;
using Village_Manager.Services.IService;

var builder = WebApplication.CreateBuilder(args);

// SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
// Add services to the container.
builder.Services.AddControllersWithViews();
// Add session support
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60); // ton tai trong 60 phut
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseStaticFiles();

app.UseRouting();

app.UseSession(); // Enable session support
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<ErrorHandlingMiddleware>();
// mac dinh khi chay
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
