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
using Microsoft.CodeAnalysis.CSharp.Syntax;

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

        [Authorize(Roles = "Admin,HR")]
        [HttpGet]
        [Route("/employees")]
        public IActionResult ListEmployees()
        {
            Console.WriteLine("Entered ListEmployees action");
            var employees = _context.HREmployees.AsNoTracking().OrderBy(e=>e.Name).ToList();
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
                    Address = emp.Address,
                    MarriageStatus = emp.MarriageStatus,
                    Religion = emp.Religion,
                    DateOfBirth = emp.DateOfBirth ?? DateTime.MinValue,
                    InsuranceNumber = emp.InsuranceNumber,
                    HireDate = emp.HireDate,
                    EndDate = emp.EndDate,
                    JobName = emp.JobName,
                    ContractType = emp.ContractType,
                    LeaveReason = emp.LeaveReason,
                    BasmaId = emp.BasmaId,
                    HRDepartmentId = emp.HRDepartmentId,
                    Department = depName == "" ? "" : depName
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
        public IActionResult AddEmployee(HREmployee newEmployee, List<IFormFile> imageFiles, string initialPassword)
        {
            if (newEmployee == null)
            {
                return Json(new { success = false, message = "بيانات الموظف غير صالحة." });
            }

            _context.HREmployees.Add(newEmployee);
            _context.Users.Add(new User
            {
                Username = newEmployee.NationalId,
                Password = PasswordHasher.HashPassword(initialPassword),
                Role = "Employee",
                IsActive = false
            });
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

            Console.WriteLine(cleanName + "❎");
            // Create ZIP in memory
            using var ms = new MemoryStream();
            using (var zip = new ZipArchive(ms, ZipArchiveMode.Create, true))
            {
                var usedNames = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
                foreach (var file in files)
                {
                    var filePath = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot",
                        file.Url.TrimStart('/')
                    );

                    if (System.IO.File.Exists(filePath))
                    {
                        var entryName = file.FileName;
                        if (usedNames.ContainsKey(entryName))
                        {
                            usedNames[entryName]++;
                            var ext = Path.GetExtension(entryName);
                            var nameWithoutExt = Path.GetFileNameWithoutExtension(entryName);
                            entryName = $"{nameWithoutExt}_{usedNames[entryName]}{ext}";
                        }
                        else
                        {
                            usedNames[entryName] = 1;
                        }

                        var entry = zip.CreateEntry(entryName);
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
            var employeeShift = _context.HREmployeeShift.FirstOrDefault(sh => sh.HREmployeeId == employeeId && DateTime.Now > sh.FromDate && sh.ToDate == null);
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

        [HttpGet]
        [Route("/getDepartmentEmployees")]
        public IActionResult GetDepartmentEmployees(int departmentId)
        {
            var employees = _context.HREmployees.Where(e => e.HRDepartmentId == departmentId).Select(e => new { e.Id, e.Name }).OrderBy(e=>e.Name).ToList();
            var DepartmentName = _context.HRDepartments.Where(d => d.Id == departmentId).Select(d => d.Name).FirstOrDefault();
            return Json(new { employees = employees, departmentName = DepartmentName });
        }


        [HttpGet]
        [Route("/getOffdaysNemployees")] /// add here all the shiftOptions if there are. upon the flag
        public IActionResult GetOffdaysNemployees(string employeeIds, DateTime startDate)
        {
            var employeeIdList = employeeIds.Split(',').Select(int.Parse).ToList();
            var offDays = _context.HREmployeeOffDays
                .Where(od => employeeIdList.Contains(od.EmployeeId) &&
                             od.OffDayDate.Date >= startDate.Date &&
                             od.OffDayDate.Date <= startDate.Date.AddDays(6))
                .GroupBy(od => od.EmployeeId)
                .Select(g => new
                {
                    EmployeeId = g.Key,
                    OffDays = g.Select(x => new { x.OffDayDate, x.OffDayType }).ToList()
                })
                .ToList(); return Json(new { offDays = offDays });
        }

        [HttpGet]
        [Route("/getShiftsForWeek")]
        public IActionResult GetShiftsForWeek(int DepartmentId, DateTime From, DateTime To)
        {
            // 1. employees in department
            var employees = _context.HREmployees
                .Where(e => e.HRDepartmentId == DepartmentId)
                .Select(e => e.Id)
                .ToList();

            // 2. default shifts
            var shifts = _context.HREmployeeShift
                .Where(s => employees.Contains(s.HREmployeeId)
                         && s.FromDate.Date <= To.Date
                         && s.ToDate.Value.Date >= From.Date)
                .ToList();

            // 3. overrides
            var overrides = _context.ShiftOverrides
                .Where(o => shifts.Select(s => s.Id).Contains(o.HREmployeeShiftId))
                .ToList();

            // 4. offs
            var offs = _context.HREmployeeOffDays
                .Where(o => employees.Contains(o.EmployeeId)
                         && o.OffDayDate.Date >= From.Date
                         && o.OffDayDate.Date <= To.Date)
                .ToList();

            var result = new List<object>();

            // 5. generate days
            for (var date = From.Date; date <= To.Date; date = date.AddDays(1))
            {
                foreach (var empId in employees)
                {
                    // OFF has priority
                    var off = offs.FirstOrDefault(o =>
                        o.EmployeeId == empId && o.OffDayDate.Date == date);

                    if (off != null)
                    {
                        result.Add(new
                        {
                            EmployeeId = empId,
                            Date = date,
                            ShiftOptionId = "off",
                            OffDayType = off.OffDayType
                        });
                        continue;
                    }

                    // default shift
                    var shift = shifts.FirstOrDefault(s =>
                        s.HREmployeeId == empId &&
                        s.FromDate.Date <= date &&
                        s.ToDate.Value.Date >= date);

                    if (shift == null)
                        continue;

                    var shiftOptionId = shift.ShiftOptionId;

                    // check override
                    var ov = overrides.FirstOrDefault(o =>
                        o.HREmployeeShiftId == shift.Id &&
                        o.DayOfWeek == (int)date.DayOfWeek);

                    if (ov != null)
                        shiftOptionId = ov.ShiftOptionId;

                    result.Add(new
                    {
                        EmployeeId = empId,
                        Date = date,
                        ShiftOptionId = shiftOptionId?.ToString(),
                        OffDayType = (string?)null
                    });
                }
            }

            return Json(result);
        }

        [HttpPost]
        [Route("/addShiftOption")]
        public IActionResult AddShiftOption(DateTime? StartTime, DateTime? EndTime, int ShiftMode, int? Hours)
        {
            // create the name so we can retrieve it later and show it to the user in the dropdown
            string FormatTime(DateTime? time)
            {
                if (!time.HasValue) return "-";

                return time.Value.ToString("hh:mm tt", new System.Globalization.CultureInfo("ar-EG"));
            }
            string name;
            if (ShiftMode == 1)
            {
                name = $"عدد ساعات: {Hours}";
            }
            else if (ShiftMode == 2)
            {
                name = $"من {FormatTime(StartTime)} إلى {FormatTime(EndTime)}";
            }
            else
            {
                name = "متغير";
            }

            // check if the same shift option already exists
            var exists = _context.HRShiftOptions.Any(s => s.ShiftMode == ShiftMode && s.StartTime == StartTime && s.EndTime == EndTime && s.RequiredHours == Hours);
            if (exists)
            {
                return Json(new { success = false, message = "Shift option already exists" });
            }

            var newShiftOption = new HRShiftOption
            {
                Name = name,
                StartTime = StartTime,
                EndTime = EndTime,
                ShiftMode = ShiftMode,
                RequiredHours = ShiftMode == 1 ? Hours : null
            };
            _context.HRShiftOptions.Add(newShiftOption);
            _context.HRLogs.Add(new HRLog
            {
                Action = $"User ({User.Identity.Name}) added shift option ({name})"
            });
            _context.SaveChanges();
            return Json(new { success = true, Name = name , ShiftOptionId = newShiftOption.Id});
        }
        [HttpGet]
        [Route("/getShiftOptions")]
        public IActionResult GetShiftOptions()
        {
            var shiftOptions = _context.HRShiftOptions.Select(s => new { s.Id, s.Name }).ToList();
            return Json(shiftOptions);
        }

        [HttpPost]
        [Route("/saveShifts")]
        public IActionResult SaveShifts([FromBody] ShiftList ShiftList)
        {
            DateTime From = ShiftList.From.Date;
            DateTime To = ShiftList.To.Date;

            // =========================
            // 1. OFF DAYS (UPSERT)
            // =========================
            var offs = ShiftList.Shifts
                .Where(x => x.ShiftOptionId == "off")
                .ToList();

            var offEmpIds = offs.Select(x => x.EmployeeId).Distinct().ToList();

            var existingOffs = _context.HREmployeeOffDays
                .Where(o => offEmpIds.Contains(o.EmployeeId)
                         && o.OffDayDate.Date >= From
                         && o.OffDayDate.Date <= To)
                .ToList();

            // remove old offs in range
            _context.HREmployeeOffDays.RemoveRange(existingOffs);

            var offEntities = offs.Select(x => new HREmployeeOffDay
            {
                EmployeeId = x.EmployeeId,
                OffDayDate = x.Date.Date,
                OffDayType = "راحة"
            }).ToList();

            _context.HREmployeeOffDays.AddRange(offEntities);

            // =========================
            // 2. WORKING SHIFTS
            // =========================
            var workingShifts = ShiftList.Shifts
                .Where(x => x.ShiftOptionId != "off")
                .ToList();

            var empIds = workingShifts
                .Select(x => x.EmployeeId)
                .Distinct()
                .ToList();

            // =========================
            // 3. GROUP + DEFAULT SHIFT
            // =========================
            var result = workingShifts
                .GroupBy(s => s.EmployeeId)
                .Select(empGroup =>
                {
                    var mostFrequentShift = empGroup
                        .GroupBy(x => x.ShiftOptionId)
                        .OrderByDescending(g => g.Count())
                        .First().Key;

                    return new
                    {
                        HREmployeeId = empGroup.Key,
                        MostFrequentShiftOptionId = mostFrequentShift,
                        Others = empGroup
                            .Where(x => x.ShiftOptionId != mostFrequentShift)
                            .ToList()
                    };
                })
                .ToList();

            // =========================
            // 4. GET EXISTING SHIFTS (UPSERT)
            // =========================
            var existingShifts = _context.HREmployeeShift
                .Where(s => empIds.Contains(s.HREmployeeId)
                         && s.FromDate.Date == From
                         && s.ToDate == To)
                .ToList();

            var existingMap = existingShifts
                .ToDictionary(x => x.HREmployeeId, x => x);

            var newShifts = new List<HREmployeeShift>();

            foreach (var emp in result)
            {
                int shiftOptionId = int.TryParse(emp.MostFrequentShiftOptionId, out int resu) ? resu : 0;

                if (existingMap.ContainsKey(emp.HREmployeeId))
                {
                    // UPDATE
                    var existing = existingMap[emp.HREmployeeId];
                    existing.ShiftOptionId = shiftOptionId;
                }
                else
                {
                    // INSERT
                    newShifts.Add(new HREmployeeShift
                    {
                        FromDate = From,
                        ToDate = To,
                        HREmployeeId = emp.HREmployeeId,
                        ShiftOptionId = shiftOptionId
                    });
                }
            }

            _context.HREmployeeShift.AddRange(newShifts);
            _context.SaveChanges();

            // =========================
            // 5. REFRESH SHIFTS (for IDs)
            // =========================
            var savedShifts = _context.HREmployeeShift
                .Where(s => empIds.Contains(s.HREmployeeId)
                         && s.FromDate.Date == From
                         && s.ToDate == To)
                .ToList();

            var shiftMap = savedShifts
                .ToDictionary(x => x.HREmployeeId, x => x);

            // =========================
            // 6. DELETE OLD OVERRIDES
            // =========================
            var shiftIds = savedShifts.Select(s => s.Id).ToList();

            var oldOverrides = _context.ShiftOverrides
                .Where(o => shiftIds.Contains(o.HREmployeeShiftId));

            _context.ShiftOverrides.RemoveRange(oldOverrides);
            _context.SaveChanges();

            // =========================
            // 7. ADD NEW OVERRIDES
            // =========================
            var overrides = new List<ShiftOverride>();

            foreach (var emp in result)
            {
                var shiftEntity = shiftMap[emp.HREmployeeId];

                foreach (var other in emp.Others)
                {
                    overrides.Add(new ShiftOverride
                    {
                        HREmployeeShiftId = shiftEntity.Id,
                        DayOfWeek = (int)other.Date.DayOfWeek,
                        ShiftOptionId = int.TryParse(other.ShiftOptionId, out int r) ? r : 0
                    });
                }
            }

            _context.ShiftOverrides.AddRange(overrides);
            _context.SaveChanges();

            // =========================
            // DONE
            // =========================
            return Json(new { success = true });
        }
    }
}