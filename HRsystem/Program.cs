using HRsystem.Data;
using HRsystem.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Infrastructure;
// using zkemkeeper;

using System.Net.Sockets;
using System.Text;
//using ZkFingerprintBridge;

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);



QuestPDF.Settings.License = LicenseType.Community;
var builder = WebApplication.CreateBuilder(args);

//// Add this with your other service registrations
//builder.Services.AddSingleton<FingerprintService>();

//// Or with configuration
//builder.Services.AddSingleton<FingerprintService>(sp =>
//    new FingerprintService("192.168.1.21", 4370));

builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/";
        options.AccessDeniedPath = "/accessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Admin"));
});
// Connection string (from appsettings.json)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// Register Notification Service
builder.Services.AddScoped<INotificationService, NotificationService>();

var app = builder.Build();

// Seed data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await DbSeeder.SeedAsync(context);
}

// Use a global exception handler
app.UseExceptionHandler("/Home/Error"); // Redirect to Error action
app.UseStatusCodePagesWithReExecute("/Home/Error", "?statusCode={0}");

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();




app.Run();