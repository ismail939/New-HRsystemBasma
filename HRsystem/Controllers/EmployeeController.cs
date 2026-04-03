using System.Diagnostics;
using System.Windows.Forms;
using HRsystem.Data;
using HRsystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using FastReport;
using Microsoft.AspNetCore.Authorization;
using System.IO.Compression;
using HRsystem.ViewModels;

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


        [Authorize(Roles = "Admin,HR")]
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

        [Authorize(Roles = "Admin,HR,Officer")]
        [HttpGet]
        [Route("/employees")]
        public IActionResult ListEmployees()
        {
            Console.WriteLine("Entered ListEmployees action");
            var employees = _context.HREmployees.ToList();
            Console.WriteLine("here is the number of the items: " + employees.Count);
            foreach (var emp in employees)
            {
                Console.WriteLine(emp.Name + " - " + emp.HRDepartmentId);
            }
            var employeeVMs = new List<EmployeeViewModel>();
            foreach (var emp in employees)
            {
                Console.WriteLine($"Processing employee: {emp.Name} with department ID: {emp.HRDepartmentId}");
                var dep = _context.HRDepartments.FirstOrDefault(d => d.Id == emp.HRDepartmentId);
                string depName = dep != null ? dep.Name : "";
                employeeVMs.Add(new EmployeeViewModel
                {
                    Id = emp.Id,
                    Name = emp.Name,
                    NationalId = emp.NationalId,
                    PhoneNumber = emp.PhoneNumber,
                    MarriageStatus = emp.MarriageStatus,
                    Religion = emp.Religion,
                    DateOfBirth = emp.DateOfBirth,
                    InsuranceNumber = emp.InsuranceNumber,
                    HireDate = emp.HireDate,
                    EndDate = emp.EndDate,
                    JobName = emp.JobName,
                    ContractType = emp.ContractType,
                    LeaveReason = emp.LeaveReason,
                    BasmaId = emp.BasmaId,
                    HRDepartmentId = emp.HRDepartmentId,
                    Department = depName==""?"": depName
                });
            }
            var departments = _context.HRDepartments.ToList();
            var employeesNdepartments = new ListEmployeesViewModel
            {
              Employees = employeeVMs,
              Departments = departments  
            };
            return View(employeesNdepartments);
        }

        [Authorize(Roles = "Admin,HR")]
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
            _context.HRLogs.Add(new HRLog
            {
                Action = $"User ({User.Identity.Name}) edited employee ({updatedEmployee.Name})"
            });
            _context.SaveChanges();
            return Json(new { success = true });
        }
        [Authorize(Roles = "Admin,HR")]
        [HttpPost]
        [Route("/employees/add")]
        public IActionResult AddEmployee(HREmployee newEmployee, List<IFormFile> imageFiles)
        {
            if (newEmployee == null)
            {
                return Json(new { success = false, message = "بيانات الموظف غير صالحة." });
            }
            
            _context.HREmployees.Add(newEmployee);
            _context.HRLogs.Add(new HRLog
            {
                Action = $"User ({User.Identity.Name}) added employee ({newEmployee.Name})"
            });
            _context.SaveChanges();
            // Handle file uploads if any
            if (imageFiles != null && imageFiles.Count > 0)
            {
                foreach (var file in imageFiles)
                {
                    string uniqueNumber = DateTime.Now.ToString("yyyyMMddHHmmssfff"); // Example: 20251026123545012
                    string extension = Path.GetExtension(file.FileName);
                    string fileName = $"{uniqueNumber}{extension}";
                    _context.HREmployeeFiles.Add(new HREmployeeFile
                    {
                        EmployeeId = newEmployee.Id,
                        FileName = file.FileName,
                        Url = "/images/" + fileName
                    });
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
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
        [HttpGet]
        [Route("employees/files/zip/{empId}")]
        public IActionResult DownloadZip(int empId)
        {
            // Fetch files for the employee
            var files = _context.HREmployeeFiles
                .Where(f => f.EmployeeId == empId)
                .ToList();

            if (!files.Any())
                return NotFound("No files found for this employee.");

            // Get employee name
            var emp = _context.HREmployees
                .FirstOrDefault(e => e.Id == empId);

            string empName = emp != null ? emp.Name : $"employee_{empId}";

            // Clean the name (remove spaces/special chars for filename)
            string cleanName = string.Concat(empName.Split(Path.GetInvalidFileNameChars())).Replace(" ", "_");

            Console.WriteLine(cleanName+"❎");
            // Create ZIP in memory
            using var ms = new MemoryStream();
            using (var zip = new ZipArchive(ms, ZipArchiveMode.Create, true))
            {
                foreach (var file in files)
                {
                    var filePath = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot",
                        file.Url.TrimStart('/')
                    );

                    if (System.IO.File.Exists(filePath))
                    {
                        var entry = zip.CreateEntry(file.FileName);
                        using var entryStream = entry.Open();
                        using var fileStream = System.IO.File.OpenRead(filePath);
                        fileStream.CopyTo(entryStream);
                    }
                }
            }

            return File(
                ms.ToArray(),
                "application/zip",
                $"{cleanName}_files.zip"  // <-- Employee name in filename
            );
        }

        [Authorize(Roles = "Admin,HR")]
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
            var Urls = new List<string>();
            foreach (var file in imageFiles)
            {
                string uniqueNumber = DateTime.Now.ToString("yyyyMMddHHmmssfff"); // Example: 20251026123545012
                string extension = Path.GetExtension(file.FileName);
                string fileName = $"{uniqueNumber}{extension}";
                _context.HREmployeeFiles.Add(new HREmployeeFile
                {
                    EmployeeId = EmployeeId,
                    FileName = file.FileName,
                    Url = "/images/" + fileName
                });
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }
                var filePath = Path.Combine(uploadsFolder, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(stream);
                }
                Urls.Add("/images/" + fileName);
            }
            _context.HRLogs.Add(new HRLog
            {
                Action = $"User ({User.Identity.Name}) uploaded files for employee ({emp.Name})"
            });
            _context.SaveChanges();
            return Json(new { success = true, Urls = Urls });
        }
        [Authorize(Roles = "Admin,HR")]
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
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", file.Url.TrimStart('/'));
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
            return Json(new { success = true });
        }

        [Authorize(Roles = "Admin,HR")]
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

        [Authorize(Roles = "Admin,HR")]
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


        [Authorize(Roles = "Admin,HR")]
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
            _context.HRLogs.Add(new HRLog
            {
                Action = $"User ({User.Identity.Name}) added rate for employeeId ({employeeRate.EmployeeId}) for month ({employeeRate.Month}/{employeeRate.Year}) with rate ({employeeRate.Rate})"
            });
            _context.SaveChanges();

            return Json(new
            {
                Success = true,
                Message = "Rate added successfully"
            });
        }

        [Authorize(Roles = "Admin,HR")]
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

        [Authorize(Roles = "Admin,HR")]
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
            _context.HRLogs.Add(new HRLog
            {
                Action = $"User ({User.Identity.Name}) added shift for employee ({employeeShift.EmployeeId}) with mode ({employeeShift.ShiftMode})"
            });
            _context.SaveChanges();
            return Json(new { success = true });
        }

        [Authorize(Roles = "Admin,HR")]
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
            _context.HRLogs.Add(new HRLog
            {
                Action = $"User ({User.Identity.Name}) added shift for employee ({employeeShift.EmployeeId}) with mode ({employeeShift.ShiftMode}) and hours ({employeeShift.RequiredHours})"
            });
            _context.SaveChanges();
            return Json(new { success = true });
        }

        [Authorize(Roles = "Admin,HR")]
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
            _context.HRLogs.Add(new HRLog
            {
                Action = $"User ({User.Identity.Name}) added shift for employee ({employeeShift.EmployeeId}) with mode ({employeeShift.ShiftMode}), start time ({employeeShift.StartTime}) and end time ({employeeShift.EndTime})"
            });
            _context.SaveChanges();
            return Json(new { success = true });
        }

    }
}