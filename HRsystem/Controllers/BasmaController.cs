using HRsystem.Data;
using HRsystem.Models;
using HRsystem.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
// using ZkFingerprintBridge;

namespace HRsystem.Controllers
{
   public class BasmaController : Controller
   {
       private readonly ILogger<BasmaController> _logger;
       private readonly AppDbContext _context;
       public BasmaController(ILogger<BasmaController> logger, AppDbContext context)
       {
           _logger = logger;
           _context = context;
       }
       [HttpGet]
       [Route("/basma")]
       public IActionResult Basma()
       {
           var list = _context.HREmployeeBasmas.ToList();
           var employees = _context.HREmployees.ToList();
           List<BasmaList> basmaList=[];
           foreach(var basma in list)
           {
               var emp = employees.FirstOrDefault(e => e.Id == basma.EmployeeId);
               if (emp != null)
               {
                   basmaList.Add(new BasmaList
                   {
                       Id = basma.Id,
                       EmployeeName = emp.Name,
                       DayDate = basma.DayDate,
                       ArrivalTime = basma.ArrivalTime,
                       DepartureTime = basma.DepartureTime,
                       TotalHours = basma.TotalHours,
                       LateMinutes = basma.LateMinutes,
                       Status = basma.Status,
                       Notes = basma.Notes
                   });
               }
           };
           return View(basmaList);
       }
       [HttpGet]
       [Route("/basmaJson")]
       public IActionResult BasmaJson()
       {
           var list = _context.HREmployeeBasmas.ToList();
           var employees = _context.HREmployees.ToList();
           List<BasmaList> basmaList = [];
           foreach (var basma in list)
           {
               var emp = employees.FirstOrDefault(e => e.Id == basma.EmployeeId);
               if (emp != null)
               {
                   basmaList.Add(new BasmaList
                   {
                       Id = basma.Id,
                       EmployeeName = emp.Name,
                       DayDate = basma.DayDate,
                       ArrivalTime = basma.ArrivalTime,
                       DepartureTime = basma.DepartureTime,
                       TotalHours = basma.TotalHours,
                       LateMinutes = basma.LateMinutes,
                       Status = basma.Status,
                       Notes = basma.Notes
                   });
               }
           }
           ;
           return Json(basmaList);
       }
       [HttpGet]
       public IActionResult SearchEmployees(string query)
       {
           query = query?.Trim() ?? "";

           var list = _context.HREmployeeBasmas.ToList();
           var employees = _context.HREmployees.ToList();

           List<BasmaList> basmaList = new List<BasmaList>();

           foreach (var basma in list)
           {
               var emp = employees.FirstOrDefault(e => e.Id == basma.EmployeeId);

               if (emp != null && emp.Name.Contains(query, StringComparison.OrdinalIgnoreCase))
               {
                   basmaList.Add(new BasmaList
                   {
                       Id = basma.Id,
                       EmployeeName = emp.Name,
                       DayDate = basma.DayDate,
                       ArrivalTime = basma.ArrivalTime,
                       DepartureTime = basma.DepartureTime,
                       TotalHours = basma.TotalHours,
                       LateMinutes = basma.LateMinutes,
                       Status = basma.Status,
                       Notes = basma.Notes
                   });
               }
           }

           return Json(basmaList);
       }

    //    [HttpGet]
    //    [Route("/getBasmaUsers")]
    //    public IActionResult BasmaUsers()
    //    {
    //        Console.WriteLine("\n========================================");
    //        Console.WriteLine("Testing ZKTeco Fingerprint Device...");
    //        Console.WriteLine("========================================\n");

    //        ZKTecoConnection zkConnection = new ZKTecoConnection();
    //        string deviceIP = "192.168.1.21"; // Change to YOUR device IP
    //        int port = 4370;
    //        if (zkConnection.ConnectToDevice(deviceIP, port))
    //        {
    //            Console.WriteLine("? Connection successful!\n");

    //            //// Get device information
    //            //zkConnection.GetDeviceInfo();

    //            //// Test beep
    //            //Console.WriteLine("\nTesting device beep...");

    //            var users = zkConnection.GetAllUsers();
    //            //if (users[1].)
    //            //// Get device status
    //            //zkConnection.GetDeviceStatus();

    //            // Get all users
    //            zkConnection.DisconnectDevice();
    //            return Json(users);
    //            //// Get attendance logs
    //            //zkConnection.GetAttendanceLogs();

    //            // Disconnect

    //        }
    //        else
    //        {
    //            Console.WriteLine("? Failed to connect to fingerprint device!");
    //            Console.WriteLine("Please check:");
    //            Console.WriteLine("  1. Device IP address is correct: " + deviceIP);
    //            Console.WriteLine("  2. Device is powered on and connected to network");
    //            Console.WriteLine("  3. Port 4370 is not blocked by firewall");
    //            Console.WriteLine("  4. Platform target is set to x86");
    //        }

    //        Console.WriteLine("\n========================================");
    //        Console.WriteLine("Fingerprint test completed.");
    //        Console.WriteLine("========================================\n");
    //        return Json(new {success=false});

    //    }
    //    [HttpGet]
    //    [Route("/basmaDailyEntries")]
    //    public IActionResult BasmaDailyEntries()
    //    {
    //        ZKTecoConnection zkConnection = new ZKTecoConnection();
    //        string deviceIP = "192.168.1.21"; // Change to YOUR device IP
    //        int port = 4370;
    //        if (zkConnection.ConnectToDevice(deviceIP, port))
    //        {
    //            Console.WriteLine("? Connection successful!\n");

    //            //// Get device information
    //            //zkConnection.GetDeviceInfo();

    //            //// Test beep
    //            //Console.WriteLine("\nTesting device beep...");

    //            var basmaEntries = zkConnection.GetAttendanceLogs();
    //            //if (users[1].)
    //            //// Get device status
    //            //zkConnection.GetDeviceStatus();

    //            // Get all users
    //            zkConnection.DisconnectDevice();
    //            return Json(basmaEntries);
    //            //// Get attendance logs
    //            //zkConnection.GetAttendanceLogs();

    //            // Disconnect

    //        }
    //        return Json(new { success = false });
    //    }

   }
}