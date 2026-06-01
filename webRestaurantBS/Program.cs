using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Negotiate;
using Microsoft.EntityFrameworkCore;
using System;
using System.Diagnostics;
using webRestaurantBS.Hubs;
using webRestaurantBS.Models;
using webRestaurantBS.Services;

static void ConfigureNativeDllPath()
{
    var arch = Environment.Is64BitProcess ? "x64" : "x86";

    var nativePath = Path.Combine(
        AppContext.BaseDirectory,
        "NativeLibs",
        arch
    );

    if (!Directory.Exists(nativePath))
        throw new DirectoryNotFoundException(nativePath);

    var path = Environment.GetEnvironmentVariable("PATH") ?? "";
    Environment.SetEnvironmentVariable("PATH", path + ";" + nativePath);
}

ConfigureNativeDllPath();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();

//builder.Services.Configure<BioStarDeviceOptions>(builder.Configuration.GetSection("BioStar:Device"));
builder.Services.Configure<BioStarOptions>(builder.Configuration.GetSection("BioStar"));

builder.Services.AddHostedService<BioStarFaceEventService>(); 
builder.Services.AddSingleton<BioStarUserResolver>();
builder.Services.AddSingleton<DeviceStateStore>(); 
builder.Services.AddHttpClient();
builder.Services.AddScoped<IMaherAfzarApiService, MaherAfzarApiService>();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication(NegotiateDefaults.AuthenticationScheme)
    .AddNegotiate();
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login"; // مسیر صفحه ورود
        options.LogoutPath = "/Account/Logout"; // مسیر صفحه خروج
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30); // مدت زمان انقضای کوکی
        options.SlidingExpiration = true; // تنظیم انقضای کشویی
    });

builder.Services.AddAuthorization();

//var cs = builder.Configuration.GetConnectionString("DefaultConnection");
//Console.WriteLine($"Connection String: {cs}");

var app = builder.Build(); 

Console.WriteLine(Environment.Is64BitProcess ? "x64 Loaded" : "x86 Loaded");
Debug.WriteLine($"Native Arch: {(Environment.Is64BitProcess ? "x64" : "x86")}");

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=FaceEvent}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");
app.MapHub<FaceEventHub>("/faceHub");

app.Run();
