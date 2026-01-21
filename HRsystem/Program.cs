using HRsystem.Data;
//using HRsystem.Services;
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

// Connection string (from appsettings.json)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();




app.Run();
