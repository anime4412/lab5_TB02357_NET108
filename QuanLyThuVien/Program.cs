using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using QuanLyThuVien.DAL;
using QuanLyThuVien.Data;
using QuanLyThuVien.Utils;

var builder = WebApplication.CreateBuilder(args);

// Cấu hình Entity Framework Core Code First với SQL Server
builder.Services.AddDbContext<LibraryDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllersWithViews();

// Đăng ký DI cho tầng DAL (Scoped) và EmailService
builder.Services.AddScoped<EmployeeDAL>();
builder.Services.AddScoped<CategoryDAL>();
builder.Services.AddScoped<BookDAL>();
builder.Services.AddTransient<EmailService>();

// Cấu hình Xác thực Cookie Authentication
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Home/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(24);
    });

// Cấu hình Phân quyền Policy
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("EmployeeOrManager", policy =>
        policy.RequireRole("Librarian", "Admin", "Employee", "Manager"));

    options.AddPolicy("ManagerOnly", policy =>
        policy.RequireRole("Admin", "Manager"));
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
