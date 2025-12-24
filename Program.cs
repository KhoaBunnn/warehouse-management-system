using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QLKhoHang.Data;
using QLKhoHang.Repositories;
using Microsoft.AspNetCore.Identity.UI.Services;
using QLKhoHang.Services;


var builder = WebApplication.CreateBuilder(args);

// 1. MVC
builder.Services.AddControllersWithViews();

// 2. DbContext
builder.Services.AddDbContext<QLKhoHangContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 3. Identity (KHÔNG yêu cầu xác thực email)
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false; // tắt xác thực email
    options.SignIn.RequireConfirmedEmail = false;   // tắt xác thực email
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 6;
    options.Password.RequireUppercase = false;
})
.AddEntityFrameworkStores<QLKhoHangContext>()
.AddDefaultTokenProviders(); // dùng token cho reset password, ... nếu cần

// Email sender rỗng (vì không dùng xác thực email)
builder.Services.AddSingleton<IEmailSender, NoEmailSender>();

builder.Services.AddRazorPages();

// Session / Cookie options
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
});

// Repository registrations
builder.Services.AddScoped<IKhoRepository, KhoRepository>();
builder.Services.AddScoped<ILoaiHangRepository, LoaiHangRepository>();
builder.Services.AddScoped<IHangHoaRepository, HangHoaRepository>();
builder.Services.AddScoped<INhanVienRepository, NhanVienRepository>();
builder.Services.AddScoped<INhaCungCapRepository, NhaCungCapRepository>();

var app = builder.Build();

// Pipeline
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

app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
// TẠO ROLE TỰ ĐỘNG KHI CHẠY HỆ THỐNG
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    string[] roles = { "Admin", "NhanVienKho", "KeToan", "QuanLy" };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    // 🔥 SEED ADMIN USER
    var adminEmail = "admin@qlkho.com";
    var adminUser = await userManager.FindByEmailAsync(adminEmail);

    if (adminUser == null)
    {
        adminUser = new IdentityUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };

        await userManager.CreateAsync(adminUser, "Admin@123");
        await userManager.AddToRoleAsync(adminUser, "Admin");
    }
}

app.Run();
