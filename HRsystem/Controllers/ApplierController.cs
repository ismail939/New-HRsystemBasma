using System.Diagnostics;
using HRsystem.Data;
using HRsystem.Models;
using Microsoft.AspNetCore.Mvc;

namespace HRsystem.Controllers
{
    public class ApplierController : Controller
    {
        private readonly ILogger<ApplierController> _logger;
        private readonly AppDbContext _context;
        public ApplierController(ILogger<ApplierController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }
        [HttpGet]
        [Route("/appliers")]
        public IActionResult ListAppliers()
        {
            var appliers = _context.HRAppliers.ToList();
            return View(appliers);
        }
        [HttpPost]
        [Route("/appliers/edit")]
        public IActionResult EditApplier(HRApplier updatedApplier)
        {
            Console.WriteLine("Received applier data: " + updatedApplier);
            if (updatedApplier == null)
            {
                return Json(new { success = false, message = "بيانات المتقدم غير صالحة." });
            }
            _context.HRAppliers.Update(updatedApplier);
            _context.SaveChanges();
            return Json(new { success = true });
        }
       
        [HttpPost]
        [Route("/appliers/add")]
        public IActionResult AddApplier(HRApplier newApplier, List<IFormFile> imageFiles)
        {
            if (newApplier == null)
            {
                return Json(new { success = false, message = "بيانات المتقدم غير صالحة." });
            }
            _context.HRAppliers.Add(newApplier);
            _context.SaveChanges();
            // Handle file uploads if any
            if (imageFiles != null && imageFiles.Count > 0)
            {
                foreach (var file in imageFiles)
                {
                    string uniqueNumber = DateTime.Now.ToString("yyyyMMddHHmmssfff"); // Example: 20251026123545012
                    string extension = Path.GetExtension(file.FileName);
                    string fileName = $"{uniqueNumber}{extension}";
                    _context.HRApplierFiles.Add(new HRApplierFile
                    {
                        ApplierId = newApplier.Id,
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
            return Json(newApplier);
        }
        [HttpPost]
        [Route("/appliers/uploadImage")]
        public IActionResult UploadImage(List<IFormFile> imageFiles, int ApplierId)
        {
            if (imageFiles == null || imageFiles.Count == 0)
            {
                return Json(new { success = false, message = "لم يتم اختيار ملف." });
            }
            // save the file url and name to the database for this applier.
            var applier = _context.HRAppliers.Find(ApplierId);
            if (applier == null)
            {
                return Json(new { success = false, message = "المتقدم غير موجود." });
            }
            List<string> Urls = [];
            foreach (var file in imageFiles)
            {
                string uniqueNumber = DateTime.Now.ToString("yyyyMMddHHmmssfff"); // Example: 20251026123545012
                string extension = Path.GetExtension(file.FileName);
                string fileName = $"{uniqueNumber}{extension}";
                _context.HRApplierFiles.Add(new HRApplierFile
                {
                    ApplierId = ApplierId,
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
            _context.SaveChanges();
            return Json(new { success = true, Urls = Urls });
        }
        [HttpGet]
        [Route("/appliers/files/{applierId}")]
        public IActionResult GetApplierFiles(int applierId)
        {
            var files = _context.HRApplierFiles
                .Where(f => f.ApplierId == applierId)
                .ToList();
            return Json(files);
        }
        [HttpPost]
        [Route("/appliers/deleteFile/{fileId}")]
        public IActionResult DeleteApplierFile(int fileId)
        {
            var file = _context.HRApplierFiles.Find(fileId);
            if (file == null)
            {
                return Json(new { success = false, message = "الملف غير موجود." });
            }
            _context.HRApplierFiles.Remove(file);
            _context.SaveChanges();
            // Optionally delete the file from the server
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", file.Url.TrimStart('/'));
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
            return Json(new { success = true });
        }
    }
}