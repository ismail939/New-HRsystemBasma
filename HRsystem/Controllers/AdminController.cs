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
        [HttpGet]
        [Route("/Admin/GetDepartmentEmployees")]
        public IActionResult GetDepartmentEmployees(int departmentId)
        {
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
            int parentDepartmentId,
            int managerId)
        {
            // Console.WriteLine($"here are the values id: {Id} name: {Name} code: {Code} description: {Description} parentDepId: {parentDepartmentId} managerId: {managerId}");
            // get the department
            var department = _context.HRDepartments.FirstOrDefault(d=>d.Id==Id);
            if (department == null)
            {
                return NotFound(); 
            }
            department.Name = Name;
            department.Code = Code;
            department.Description = Description;
            department.ParentDepartmentId = parentDepartmentId;
            department.ManagerId = managerId;
            _context.HRDepartments.Update(department);
            _context.SaveChanges();
            return RedirectToAction("Departments");
        }

    }
}