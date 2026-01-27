using System.Diagnostics;
using System.Windows.Forms;
using HRsystem.Data;
using HRsystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using FastReport;

namespace HRsystem.Controllers
{
    public class EmployeeController : Controller
    {
        private readonly ILogger<EmployeeController> _logger;
        private readonly AppDbContext _context;
        public EmployeeController(ILogger<EmployeeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        [Route("/getEmployee")]
        public IActionResult GetEmployee(int employeeId)
        {
            var employee = _context.HREmployees.FirstOrDefault(e => e.Id == employeeId);
            if (employee != null)
            {
                return Json(employee);
            }
            return Json(new { success = false });
        }
        [HttpGet]
        [Route("/employees")]
        public IActionResult ListEmployees()
        {
            var employees = _context.HREmployees.ToList();
            return View(employees);
        }
        [HttpPost]
        [Route("/employees/edit")]
        public IActionResult EditEmployee(HREmployee updatedEmployee)
        {
            Console.WriteLine("Received employee data: " + updatedEmployee);
            if (updatedEmployee == null)
            {
                return Json(new { success = false, message = "بيانات الموظف غير صالحة." });
            }
            _context.HREmployees.Update(updatedEmployee);
            _context.SaveChanges();
            return Json(new { success = true });
        }

        [HttpPost]
        [Route("/employees/add")]
        public IActionResult AddEmployee(HREmployee newEmployee, List<IFormFile> imageFiles)
        {
            if (newEmployee == null)
            {
                return Json(new { success = false, message = "بيانات الموظف غير صالحة." });
            }
            _context.HREmployees.Add(newEmployee);
            _context.SaveChanges();
            // Handle file uploads if any
            if (imageFiles != null && imageFiles.Count > 0)
            {
                foreach (var file in imageFiles)
                {
                    string uniqueNumber = DateTime.Now.ToString("T"); // Example: 
                    string extension = Path.GetExtension(file.FileName);
                    string fileName = $"{uniqueNumber}{file.FileName}{extension}";
                    _context.HREmployeeFiles.Add(new HREmployeeFile
                    {
                        EmployeeId = newEmployee.Id,
                        FileName = file.FileName,
                        Url = $"/Uploads/{newEmployee.Name}" + fileName
                    });
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", $"{newEmployee.Name}");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }
                    var filePath = Path.Combine(uploadsFolder, fileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(stream);
                    }
                }
                _context.SaveChanges();
            }
            return Json(newEmployee);
        }
        [HttpPost]
        [Route("/employees/uploadImage")]
        public IActionResult UploadImage(List<IFormFile> imageFiles, int EmployeeId)
        {
            if (imageFiles == null || imageFiles.Count == 0)
            {
                return Json(new { success = false, message = "لم يتم اختيار ملف." });
            }
            // save the file url and name to the database for this employee.
            var emp = _context.HREmployees.Find(EmployeeId);
            if (emp == null)
            {
                return Json(new { success = false, message = "الموظف غير موجود." });
            }
            List<string> Urls = [];
            foreach (var file in imageFiles)
            {
                string safeName = Path.GetFileName(file.FileName);
                string fileName = $"{Guid.NewGuid()}_{safeName}";
                Console.WriteLine($"❎/Uploads/{emp.Name}/" + fileName);

                _context.HREmployeeFiles.Add(new HREmployeeFile
                {
                    EmployeeId = emp.Id,
                    FileName = file.FileName,
                    Url = $"/Uploads/{emp.Name}/" + fileName
                });
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", $"{emp.Name}");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
                Console.WriteLine(uploadsFolder + "👈uploadsFolder");
                var filePath = Path.Combine(uploadsFolder, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
                Console.WriteLine(filePath + "👈filePath");
                Console.WriteLine($"/Uploads/{emp.Name}/" + fileName + "👈file url");
                Urls.Add($"/Uploads/{emp.Name}/" + fileName);
            }
            _context.SaveChanges();
            return Json(new { success = true, Urls = Urls });
        }
        [HttpGet]
        [Route("/employees/files/{employeeId}")]
        public IActionResult GetEmployeeFiles(int employeeId)
        {
            Console.WriteLine(employeeId + "❎");
            var files = _context.HREmployeeFiles
                .Where(f => f.EmployeeId == employeeId)
                .ToList();
            return Json(files);
        }
        [HttpPost]
        [Route("/employees/deleteFile/{fileId}")]
        public IActionResult DeleteEmployeeFile(int fileId)
        {
            var file = _context.HREmployeeFiles.Find(fileId);
            if (file == null)
            {
                return Json(new { success = false, message = "الملف غير موجود." });
            }
            _context.HREmployeeFiles.Remove(file);
            _context.SaveChanges();
            // Optionally delete the file from the server
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", file.Url.TrimStart('/'));
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
            return Json(new { success = true });
        }
        [HttpGet]
        [Route("/downloadFile/{*filePath}")]
        public IActionResult DownloadFile(string filePath)
        {
            try
            {
                // Security: Validate filePath to prevent directory traversal
                if (string.IsNullOrEmpty(filePath) || filePath.Contains(".."))
                    return BadRequest("Invalid file path");

                // Decode the URL-encoded path (for Arabic names)
                filePath = Uri.UnescapeDataString(filePath);

                // Build full path to file in uploads folder
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
                var fullFilePath = Path.Combine(uploadsFolder, filePath);

                Console.WriteLine($"Uploads Folder: {uploadsFolder}");
                Console.WriteLine($"Full File Path: {fullFilePath}");
                // string rootPath = _env.ContentRootPath;

                // string physicalPath = Path.Combine(
                //     rootPath,
                //     "Uploads",
                //     "كريم حسن إبراهيم",
                //     "266f4290-ab2b-4d01-b553-bba8d8010f15_C2N5wXQ.jpg"
                // );

                // Console.WriteLine(File.Exists(physicalPath)); // works fine

                // Check if file exists
                if (!System.IO.File.Exists(fullFilePath))
                    return NotFound("File not found");

                // Get file bytes
                var fileBytes = System.IO.File.ReadAllBytes(fullFilePath);
                var contentType = GetContentType(fullFilePath);
                var fileName = Path.GetFileName(fullFilePath);

                return File(fileBytes, contentType, fileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error: {ex.Message}");
            }
        }

        private string GetContentType(string filePath)
        {
            var ext = Path.GetExtension(filePath).ToLower();
            return ext switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".pdf" => "application/pdf",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                _ => "application/octet-stream"
            };
        }
        [HttpGet]
        [Route("/getRate")]
        public IActionResult GetRate(int idForCurEmp, int month, int year)
        {
            Console.WriteLine($"i entered getRate action");
            var rate = _context.HREmployeeRates.Where(rate => rate.Rate != 0 && rate.Month == month && rate.Year == year && rate.EmployeeId == idForCurEmp).FirstOrDefault();
            Console.WriteLine($"rate is {rate}");
            var result = new JsonResult { Success = true, Data = rate };
            if (rate == null)
            {
                result.Success = false;
            }
            Console.WriteLine($"result is {result.Success}");
            return Json(result);
        }



        [HttpPost]
        [Route("/addRate")]
        public IActionResult AddRate(decimal rate, int employeeId, int month, int year)
        {
            var exists = _context.HREmployeeRates.Any(r =>
            r.EmployeeId == employeeId &&
            r.Month == month &&
            r.Year == year
            );

            if (exists)
            {
                return Json(new
                {
                    Success = false,
                    Message = "Rate already exists for this month"
                });
            }

            // تحقق بسيط
            if (rate < 1 || rate > 5)
                return BadRequest(new { Success = false, Message = "Invalid rate value" });

            var employeeRate = new HREmployeeRate
            {
                EmployeeId = employeeId,
                Rate = rate,
                Month = month,
                Year = year,
            };

            _context.HREmployeeRates.Add(employeeRate);
            _context.SaveChanges();

            return Json(new
            {
                Success = true,
                Message = "Rate added successfully"
            });
        }

        [HttpGet]
        [Route("/getShift")]
        public IActionResult GetShift(int employeeId)
        {
            var employeeShift = _context.HREmployeeShift.FirstOrDefault(sh => sh.EmployeeId == employeeId && DateTime.Now > sh.FromDate && sh.ToDate == null);
            // check if empshift is good or no
            if (employeeShift != null)
            {
                return Json(new { success = true, shift = employeeShift });
            }
            else
            {
                return Json(new { success = false });
            }
        }

        [HttpPost]
        [Route("/addShiftVariable")]
        public IActionResult AddShiftVariable(int EmployeeId)
        {
            // check if there is a shift for this emp with the toDate = null and make the toDate the datetime.now
            var updatedShift = _context.HREmployeeShift.FirstOrDefault(shift => shift.EmployeeId == EmployeeId && shift.ToDate == null);
            if (updatedShift != null)
            {
                updatedShift.ToDate = DateTime.Now;
                _context.SaveChanges();
            }
            var employeeShift = new HREmployeeShift
            {
                ShiftMode = 0,
                FromDate = DateTime.Now,
                EmployeeId = EmployeeId
            };
            _context.HREmployeeShift.Add(employeeShift);
            _context.SaveChanges();
            return Json(new { success = true });
        }
        [HttpPost]
        [Route("/addShiftHours")]
        public IActionResult AddShift(int EmployeeId, int Hours)
        {
            // check if there is a shift for this emp with the toDate = null and make the toDate the datetime.now
            var updatedShift = _context.HREmployeeShift.FirstOrDefault(shift => shift.EmployeeId == EmployeeId && shift.ToDate == null);
            if (updatedShift != null)
            {
                updatedShift.ToDate = DateTime.Now;
            }
            _context.SaveChanges();
            var employeeShift = new HREmployeeShift
            {
                RequiredHours = Hours,
                ShiftMode = 1,
                FromDate = DateTime.Now,
                EmployeeId = EmployeeId
            };
            _context.HREmployeeShift.Add(employeeShift);
            _context.SaveChanges();
            return Json(new { success = true });
        }
        [HttpPost]
        [Route("/addShiftFixed")]
        public IActionResult AddShiftFixed(int EmployeeId, DateTime StartTime, DateTime EndTime)
        {
            // check if there is a shift for this emp with the toDate = null and make the toDate the datetime.now
            var updatedShift = _context.HREmployeeShift.FirstOrDefault(shift => shift.EmployeeId == EmployeeId && shift.ToDate == null);
            if (updatedShift != null)
            {
                updatedShift.ToDate = DateTime.Now;
            }
            _context.SaveChanges();
            var employeeShift = new HREmployeeShift
            {
                StartTime = StartTime,
                EndTime = EndTime,
                ShiftMode = 2,
                FromDate = DateTime.Now,
                EmployeeId = EmployeeId
            };
            _context.HREmployeeShift.Add(employeeShift);
            _context.SaveChanges();
            return Json(new { success = true });
        }

    }
}