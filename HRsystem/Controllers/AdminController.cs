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
            foreach(var department in listD)
            {
                // get managerName
                var emp = _context.HREmployees.FirstOrDefault(e=>e.Id == department.ManagerId);
                if(emp == null)
                {
                    return Json(new {success= false, Message = "No employee with this id was found"});
                }
                var ManagerName = emp.Name;
                // get parentDepartmentName
                var parentDepartment = _context.HRDepartments.FirstOrDefault(d=>d.Id == department.ParentDepartmentId);
               
                var ParentDepartmentName = (parentDepartment!=null)?parentDepartment.Name: "";
                DepartmentViewModel depVM = new DepartmentViewModel
                {
                  Name = department.Name,
                  Code = department.Code,
                  Description = department.Description?? "",
                  ManagerName = ManagerName,
                  ParentDepartmentName = ParentDepartmentName,
                  ManagerId = department.ManagerId,
                  ParentDepartmentId = department.ParentDepartmentId,
                };
                list.Add(depVM);
            }
            return View("Departments", list);
        }

        // [HttpGet]
        // [Route("/Admin/GetDepartmentEmployees")]
        // public IActionResult GetDepartmentEmployees(int departmentId)
        // {
        //     var employees = _context.HREmployees
        //         .Where(e => e.DepartmentId == departmentId)
        //         .Select(e => new { Id = e.Id, Name = e.Name })
        //         .ToList();

        //     return Json(employees);
        // }

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

            return RedirectToAction("CreateForm");
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


    }
}