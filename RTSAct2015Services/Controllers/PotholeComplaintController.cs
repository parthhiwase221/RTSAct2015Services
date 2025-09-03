using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using RTSAct2015Services.Interfaces.IServices;
using RTSAct2015Services.Models.DTOs;

namespace RTSAct2015Services.Controllers
{
    public class PotholeComplaintController : Controller
    {
        private readonly IPotholeComplaintService _service;
        private readonly ILogger<PotholeComplaintController> _logger;
        private readonly IWebHostEnvironment _env;

        public PotholeComplaintController(
            IPotholeComplaintService service,
            ILogger<PotholeComplaintController> logger,
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
        public async Task<IActionResult> Create([FromForm] PotholeComplaintCreateDto model)
        {
            try
            {
                _logger.LogInformation("Pothole complaint form submitted - Title: {Title}, FirstName: {FirstName}, Mobile: {Mobile}, RoadName: {RoadName}, PotholeCount: {PotholeCount}",
                    model.Title, model.FirstName, model.Mobile, model.RoadName, model.PotholeCount);

                if (!ModelState.IsValid)
                {
                    var errors = new List<string>();

                    foreach (var kvp in ModelState)
                    {
                        if (kvp.Value.Errors.Count > 0)
                        {
                            foreach (var error in kvp.Value.Errors)
                            {
                                errors.Add($"{kvp.Key}: {error.ErrorMessage}");
                                _logger.LogWarning("ModelState Error - Field: {Field}, Error: {Error}", kvp.Key, error.ErrorMessage);
                            }
                        }
                    }

                    _logger.LogWarning("Pothole complaint validation failed with {ErrorCount} errors", errors.Count);

                    return Json(new
                    {
                        success = false,
                        message = "कृपया खालील त्रुटी दुरुस्त करा:",
                        errors = errors
                    });
                }

                _logger.LogInformation("ModelState validation passed. Calling service...");

                var result = await _service.CreateApplicationAsync(model);

                _logger.LogInformation("Service call completed - Success: {Success}, Message: '{Message}'",
                    result.Success, result.Message);

                if (result.Success && result.Data != null)
                {
                    _logger.LogInformation("Pothole complaint created successfully: {ApplicationId}", result.Data.ApplicationID);

                    return Json(new
                    {
                        success = true,
                        message = "खड्डे भरणे तक्रार यशस्वीरित्या सबमिट झाली!",
                        data = new
                        {
                            applicationId = result.Data.ApplicationID,
                            status = result.Data.Status,
                            message = result.Data.Message
                        }
                    });
                }

                _logger.LogError("Pothole complaint service returned failure: {Message}", result.Message);
                return Json(new
                {
                    success = false,
                    message = result.Message ?? "तक्रार सबमिट करण्यात अपयश. कृपया पुन्हा प्रयत्न करा.",
                    errors = new[] { result.Message ?? "Service error" }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in pothole complaint controller");
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
                _logger.LogError(ex, "Error getting pothole complaint by ID: {Id}", id);
                return Json(new { success = false, message = "Error retrieving application" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> List(int pageNumber = 1, int pageSize = 10, string? status = null, string? priority = null, string? searchText = null)
        {
            try
            {
                var result = await _service.GetApplicationsListAsync(pageNumber, pageSize, status, priority, searchText);
                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pothole complaints list");
                return Json(new { success = false, message = "Error retrieving applications list" });
            }
        }
    }
}
