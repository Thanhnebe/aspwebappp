using Microsoft.EntityFrameworkCore;
using MyMvcApp.Data;

var builder = WebApplication.CreateBuilder(args);

// Thêm dịch vụ session
builder.Services.AddSession();
builder.Services.AddControllersWithViews();

// Cấu hình DbContext với SQL Server
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

var app = builder.Build();

// Cấu hình pipeline xử lý yêu cầu HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseSession(); // Đặt đúng vị trí
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");

    endpoints.MapControllerRoute(
        name: "admin",
        pattern: "Admin/{action=Index}/{id?}",
        defaults: new { controller = "Admin", action = "Index" });
});

app.Run();
