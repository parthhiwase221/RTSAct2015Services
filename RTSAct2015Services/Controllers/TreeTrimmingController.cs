using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using RTSAct2015Services.Interfaces.IServices;
using RTSAct2015Services.Models.DTOs;
using RTSAct2015Services.Models.Entities;
using System.IO;
using System.Threading.Tasks;

namespace RTSAct2015Services.Controllers
{
    public class TreeTrimmingController : Controller
    {
        private readonly ITreeTrimmingService _service;
        private readonly ILogger<TreeTrimmingController> _logger;
        private readonly IWebHostEnvironment _env;

        public TreeTrimmingController(
            ITreeTrimmingService service,
            ILogger<TreeTrimmingController> logger,
            IWebHostEnvironment env)
        {
            _service = service;
            _logger = logger;
            _env = env;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] TreeTrimmingCreateDto model)
        {
            try
            {
                // **CUSTOM CHECKBOX VALIDATION**
                if (!model.TermsCondition1)
                {
                    ModelState.AddModelError("TermsCondition1", "You must accept all terms and conditions");
                }

                if (!model.TermsCondition2)
                {
                    ModelState.AddModelError("TermsCondition2", "You must accept all terms and conditions");
                }

                if (!model.TermsCondition3)
                {
                    ModelState.AddModelError("TermsCondition3", "You must accept all terms and conditions");
                }

                if (!model.TermsCondition4)
                {
                    ModelState.AddModelError("TermsCondition4", "You must accept all terms and conditions");
                }

                if (!model.TermsCondition5)
                {
                    ModelState.AddModelError("TermsCondition5", "You must accept all terms and conditions");
                }

                if (!model.TermsCondition6)
                {
                    ModelState.AddModelError("TermsCondition6", "You must accept all terms and conditions");
                }

                if (!ModelState.IsValid)
                {
                    var errors = new List<string>();

                    foreach (var modelError in ModelState.Values)
                    {
                        foreach (var error in modelError.Errors)
                        {
                            errors.Add(error.ErrorMessage);
                        }
                    }

                    _logger.LogWarning("Tree Trimming validation failed: {Errors}", string.Join(", ", errors));

                    return Json(new
                    {
                        success = false,
                        message = "कृपया खालील त्रुटी दुरुस्त करा:",
                        errors = errors
                    });
                }

                var result = await _service.CreateApplicationAsync(model);

                if (result.Success && result.Data != null)
                {
                    _logger.LogInformation("Tree trimming application created: {ApplicationId}", result.Data.ApplicationID);

                    return Json(new
                    {
                        success = true,
                        message = "वृक्ष छाटणी अर्ज यशस्वीरित्या सबमिट झाला!",
                        data = new
                        {
                            applicationId = result.Data.ApplicationID,
                            status = result.Data.Status,
                            message = result.Data.Message
                        }
                    });
                }

                _logger.LogError("Tree Trimming service returned failure: {Message}", result.Message);
                return Json(new
                {
                    success = false,
                    message = result.Message ?? "अर्ज सबमिट करण्यात अपयश. कृपया पुन्हा प्रयत्न करा.",
                    errors = new[] { result.Message ?? "Service error" }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in tree trimming controller");
                return Json(new
                {
                    success = false,
                    message = "अनपेक्षित त्रुटी आली. कृपया पुन्हा प्रयत्न करा.",
                    errors = new[] { "Server error" }
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetById(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return Json(new { success = false, message = "Application ID is required" });
            }

            try
            {
                var result = await _service.GetApplicationByIdAsync(id);
                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting tree trimming application by ID: {Id}", id);
                return Json(new { success = false, message = "Error retrieving application" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> List(
            int pageNumber = 1,
            int pageSize = 10,
            string? status = null,
            string? priority = null,
            string? searchText = null)
        {
            try
            {
                var result = await _service.GetApplicationsListAsync(pageNumber, pageSize, status, priority, searchText);
                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting tree trimming applications list");
                return Json(new { success = false, message = "Error retrieving applications list" });
            }
        }

        [HttpGet]
        public IActionResult DownloadNOCTemplate()
        {
            try
            {
                var webRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                var filePath = Path.Combine(webRoot, "documents", "NOC-Letter-Tree-Garden-Dept.pdf");

                if (!System.IO.File.Exists(filePath))
                {
                    _logger.LogWarning("NOC template file not found at path: {FilePath}", filePath);
                    return NotFound("NOC template not found");
                }

                var fileBytes = System.IO.File.ReadAllBytes(filePath);
                return File(fileBytes, "application/pdf", "NOC-Letter-Template.pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error downloading NOC template");
                return BadRequest("Error downloading NOC template");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(string id, [FromForm] TreeTrimmingCreateDto model)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return Json(new { success = false, message = "Application ID is required" });
                }

                var result = await _service.UpdateApplicationAsync(id, model);

                if (result.Success)
                {
                    _logger.LogInformation("Tree trimming application updated: {ApplicationId}", id);

                    return Json(new
                    {
                        success = true,
                        message = "वृक्ष छाटणी अर्ज यशस्वीरित्या अपडेट झाला!",
                        data = new { applicationId = id }
                    });
                }

                return Json(new
                {
                    success = false,
                    message = result.Message ?? "अर्ज अपडेट करण्यात अपयश",
                    errors = new[] { result.Message ?? "Update failed" }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception updating tree trimming application: {Id}", id);
                return Json(new
                {
                    success = false,
                    message = "अनपेक्षित त्रुटी आली. कृपया पुन्हा प्रयत्न करा.",
                    errors = new[] { "Update error" }
                });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    return Json(new { success = false, message = "Application ID is required" });
                }

                var result = await _service.DeleteApplicationAsync(id, "System");

                if (result.Success)
                {
                    _logger.LogInformation("Tree trimming application deleted: {ApplicationId}", id);

                    return Json(new
                    {
                        success = true,
                        message = "वृक्ष छाटणी अर्ज यशस्वीरित्या डिलीट झाला!",
                        data = new { applicationId = id }
                    });
                }

                return Json(new
                {
                    success = false,
                    message = result.Message ?? "अर्ज डिलीट करण्यात अपयश",
                    errors = new[] { result.Message ?? "Delete failed" }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception deleting tree trimming application: {Id}", id);
                return Json(new
                {
                    success = false,
                    message = "अनपेक्षित त्रुटी आली. कृपया पुन्हा प्रयत्न करा.",
                    errors = new[] { "Delete error" }
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            try
            {
                var result = await _service.GetApplicationsListAsync(1, 10);
                ViewBag.RecentApplications = result.Data ?? new List<ApplicationEntity>();
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading tree trimming dashboard");
                ViewBag.RecentApplications = new List<ApplicationEntity>();
                return View();
            }
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
    }
}
