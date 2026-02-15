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
            return View("Departments");
        }
        [HttpGet]
        [Route("/createDepartment")]
        public IActionResult Create()
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DepartmentPostRequest request)
        {
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

            return Json(new { success = true });
        }


    }
}