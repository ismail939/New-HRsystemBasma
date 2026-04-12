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
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Specialized;
using HRsystem.ViewModels;

namespace HRsystem.Controllers
{
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;
        private readonly AppDbContext _context;
        public AdminController(ILogger<AdminController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet]
        [Route("/admin/dashboard")]
        public IActionResult Dashboard()
        {
            return View();
        }
        [HttpGet]
        [Route("/admin/users")]
        public IActionResult Users()
        {
            var users = _context.Users.ToList();
            return View(users);
        }
        [HttpGet]
        [Route("/admin/dashboard/departments")]
        public IActionResult Departments()
        {
            var listD = _context.HRDepartments.ToList();
            List<DepartmentViewModel> list = [];
            foreach (var department in listD)
            {
                DepartmentViewModel depVM = null;
                // get managerName
                if (department.ManagerId == null)
                {
                    depVM = new DepartmentViewModel
                    {
                        Id = department.Id,
                        Name = department.Name,
                        Code = department.Code,
                        Description = department.Description ?? "",
                        ManagerName = "",
                        ParentDepartmentName = "",
                        ManagerId = null,
                        ParentDepartmentId = department.ParentDepartmentId,
                    };
                }
                else
                {
                    var emp = _context.HREmployees.FirstOrDefault(e => e.Id == department.ManagerId);
                    if (emp == null)
                    {
                        return Json(new { success = false, Message = "No employee with this id was found" });
                    }
                    var ManagerName = emp.Name;
                    // get parentDepartmentName
                    var parentDepartment = _context.HRDepartments.FirstOrDefault(d => d.Id == department.ParentDepartmentId);

                    var ParentDepartmentName = (parentDepartment != null) ? parentDepartment.Name : "";
                    depVM = new DepartmentViewModel
                    {
                        Id = department.Id,
                        Name = department.Name,
                        Code = department.Code,
                        Description = department.Description ?? "",
                        ManagerName = ManagerName,
                        ParentDepartmentName = ParentDepartmentName,
                        ManagerId = department.ManagerId,
                        ParentDepartmentId = department.ParentDepartmentId,
                    };
                }


                list.Add(depVM);
            }
            return View("Departments", list);
        }
        [HttpGet]
        [Route("/admin/employees")]
        public IActionResult Employees()
        {
            Console.WriteLine("Entered ListEmployees action");
            var employees = _context.HREmployees.ToList();

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
                    Department = depName == "" ? "" : depName
                });
            }

            return View("Employees", employeeVMs);
        }
        [HttpPost]
        [Route("/deleteEmployee")]
        public async Task<IActionResult> DeleteEmployee(int employeeId)
        {
            var emp = _context.HREmployees.FirstOrDefault(e => e.Id == employeeId);
            if (emp != null)
            {
                // delete all basmas
                var basmasIds = _context.HREmployeeBasmas.Where(b => b.EmployeeId == emp.Id).Select(b => b.Id).ToList();
                var items = _context.HREmployeeBasmas
                    .Where(x => basmasIds.Contains(x.Id))
                    .ToList();
                _context.HREmployeeBasmas.RemoveRange(items);
                _context.SaveChanges();
                // delete all offdays
                var offdaysIds = _context.HREmployeeOffDays.Where(b => b.EmployeeId == emp.Id).Select(b => b.Id).ToList();
                var items2 = _context.HREmployeeOffDays
                    .Where(x => offdaysIds.Contains(x.Id))
                    .ToList();
                _context.HREmployeeOffDays.RemoveRange(items2);
                _context.SaveChanges();
                // delete all penalties
                var penaltiesIds = _context.HREmployeePenalties.Where(b => b.EmployeeId == emp.Id).Select(b => b.Id).ToList();
                var items3 = _context.HREmployeePenalties
                    .Where(x => penaltiesIds.Contains(x.Id))
                    .ToList();
                _context.HREmployeePenalties.RemoveRange(items3);
                _context.SaveChanges();
                // shift 
                var shiftsId = _context.HREmployeeShift.Where(b => b.EmployeeId == emp.Id).Select(b => b.Id).ToList();
                var items4 = _context.HREmployeeShift
                    .Where(x => shiftsId.Contains(x.Id))
                    .ToList();
                _context.HREmployeeShift.RemoveRange(items4);
                _context.SaveChanges();
                // rate
                var rateIds = _context.HREmployeeRates.Where(b => b.EmployeeId == emp.Id).Select(b => b.Id).ToList();
                var items5 = _context.HREmployeeRates
                    .Where(x => rateIds.Contains(x.Id))
                    .ToList();
                _context.HREmployeeRates.RemoveRange(items5);
                _context.SaveChanges();
                // files
                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    // 1. Get all files for this employee
                    var files = await _context.HREmployeeFiles
                        .Where(f => f.EmployeeId == employeeId)
                        .ToListAsync();

                    // 2. Delete files from storage
                    foreach (var file in files)
                    {
                        if (!string.IsNullOrEmpty(file.Url))
                        {
                            // file.Url includes "images/filename.jpg"
                            var relativePath = file.Url.TrimStart('/', '\\'); // remove starting slash if present

                            var fullPath = Path.Combine(
                                Directory.GetCurrentDirectory(),
                                "wwwroot",
                                relativePath
                            );

                            if (System.IO.File.Exists(fullPath))
                            {
                                System.IO.File.Delete(fullPath);
                            }
                        }
                    }

                    // 3. Delete from database
                    _context.HREmployeeFiles.RemoveRange(files);
                    await _context.SaveChangesAsync();

                    // 4. Commit transaction
                    await transaction.CommitAsync();
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync();
                    throw;
                }
                var department = _context.HRDepartments.FirstOrDefault(d=>d.ManagerId==emp.Id);
                if (department!=null)
                {
                    department.ManagerId=null;
                    _context.HRDepartments.Update(department);
                    _context.SaveChanges();
                }
                _context.HREmployees.Remove(emp);
                _context.SaveChanges();
            }
            return Json(new {success=true});
        }
        [HttpGet]
        [Route("/Admin/GetDepartmentEmployees")]
        public IActionResult GetDepartmentEmployees(int departmentId)
        {
            Console.WriteLine($"the departmentId i got is = {departmentId}");
            var employees = _context.HREmployees
                .Where(e => e.HRDepartmentId == departmentId)
                .Select(e => new { Id = e.Id, Name = e.Name })
                .ToList();
            
            return Json(employees);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DepartmentPostRequest request)
        {
            Console.WriteLine("I am in Create Post action");
            if (!ModelState.IsValid)
            {
                request.Departments = _context.HRDepartments
                    .Select(d => new SelectListItem
                    {
                        Value = d.Id.ToString(),
                        Text = d.Name
                    })
                    .ToList();

                request.Managers = _context.HREmployees
                    .Select(e => new SelectListItem
                    {
                        Value = e.Id.ToString(),
                        Text = e.Name
                    })
                    .ToList();

                return View("CreateDepartment", request);
            }

            var department = new HRDepartment
            {
                Name = request.Name,
                Code = request.Code,
                Description = request.Description,
                ParentDepartmentId = request.ParentDepartmentId,
                ManagerId = request.ManagerId
            };

            _context.HRDepartments.Add(department);
            await _context.SaveChangesAsync();

            return RedirectToAction("Departments");
        }
        [HttpGet]
        [Route("/createDepartment")]
        public IActionResult CreateForm()
        {
            var model = new DepartmentPostRequest
            {
                Departments = _context.HRDepartments
                    .Select(d => new SelectListItem
                    {
                        Value = d.Id.ToString(),
                        Text = d.Name
                    })
                    .ToList(), // ✅ IMPORTANT

                Managers = _context.HREmployees
                    .Select(e => new SelectListItem
                    {
                        Value = e.Id.ToString(),
                        Text = e.Name
                    })
                    .ToList() // ✅ IMPORTANT
            };

            return View("CreateDepartment", model);
        }

        [HttpPost]
        public IActionResult EditDepartment(int Id,
            string Name,
            string Code,
            string Description,
            int? parentDepartmentId,
            int managerId)
        {
            // Console.WriteLine($"here are the values id: {Id} name: {Name} code: {Code} description: {Description} parentDepId: {parentDepartmentId} managerId: {managerId}");
            // get the department
            var department = _context.HRDepartments.FirstOrDefault(d => d.Id == Id);
            if (department == null)
            {
                return NotFound();
            }
            Console.WriteLine("***************");
            Console.WriteLine(managerId);
            Console.WriteLine("***************");
            department.Name = Name;
            department.Code = Code;
            department.Description = Description;
            department.ParentDepartmentId = parentDepartmentId;
            department.ManagerId = managerId;
            _context.HRDepartments.Update(department);
            _context.SaveChanges();
            return RedirectToAction("Departments");
        }

        [HttpPost]
        public IActionResult EditUser(int Id,
            string Username,
            string Password,
            string Role)
        {            
            var user = _context.Users.FirstOrDefault(u => u.Id == Id);
            if (user == null)
            {
                return NotFound();
            }
            var existingUser = _context.Users.FirstOrDefault(u => u.Username == Username && u.Id != Id);
            if (existingUser != null)            {
                ModelState.AddModelError("", "This username is already taken.");
                var users = _context.Users.ToList();
                return View("Users", users);
            }
            user.Username = Username;
            user.Password = Password;
            user.Role = Role;
            _context.Users.Update(user);
            _context.SaveChanges();
            return RedirectToAction("Users");
        }

        [HttpPost]
        public IActionResult AddUser(string Username, string Password, string Role)
        {
            var existingUser = _context.Users.FirstOrDefault(u => u.Username == Username);
            if (existingUser != null)
            {
                ModelState.AddModelError("", "This username is already taken.");
                var users = _context.Users.ToList();
                return View("Users", users);
            }
            var user = new User
            {
                Username = Username,
                Password = Password,
                Role = Role
            };
            _context.Users.Add(user);
            _context.SaveChanges();
            return RedirectToAction("Users");
        }

        [HttpGet]
        [Route("/logs")]
        public IActionResult Logs()
        {
            var logs = _context.HRLogs
                .OrderByDescending(l => l.CreatedAt)
                .ToList();

            return View("Logs", logs);
        }
    }

}