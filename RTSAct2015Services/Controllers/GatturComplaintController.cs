using Microsoft.AspNetCore.Mvc;
using RTSAct2015Services.Interfaces.IServices;
using RTSAct2015Services.Models.DTOs;

namespace RTSAct2015Services.Controllers
{
    public class GatturComplaintController : Controller
    {
        private readonly IGatturComplaintService _service;
        private readonly ILogger<GatturComplaintController> _logger;

        public GatturComplaintController(
            IGatturComplaintService service,
            ILogger<GatturComplaintController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] GatturComplaintCreateDto model)
        {
            try
            {
                _logger.LogInformation("Gattur complaint form submitted - Title: {Title}, FirstName: {FirstName}, Mobile: {Mobile}, ZoneType: {ZoneType}",
                    model.Title, model.FirstName, model.Mobile, model.ZoneType);

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

                    _logger.LogWarning("Gattur complaint validation failed with {ErrorCount} errors", errors.Count);

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
                    _logger.LogInformation("Gattur complaint created successfully: {ApplicationId}", result.Data.ApplicationID);

                    return Json(new
                    {
                        success = true,
                        message = "गटार तक्रार यशस्वीरित्या सबमिट झाली!",
                        data = new
                        {
                            gutComplaintId = result.Data.ApplicationID,  // ✅ Fixed: Use ApplicationID as complaint ID
                            applicationId = result.Data.ApplicationID,
                            status = result.Data.Status,
                            message = result.Data.Message
                        }
                    });
                }

                _logger.LogError("Gattur complaint service returned failure: {Message}", result.Message);
                return Json(new
                {
                    success = false,
                    message = result.Message ?? "तक्रार सबमिट करण्यात अपयश. कृपया पुन्हा प्रयत्न करा.",
                    errors = new[] { result.Message ?? "Service error" }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in gattur complaint controller");
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
                _logger.LogError(ex, "Error getting gattur complaint by ID: {Id}", id);
                return Json(new { success = false, message = "Error retrieving application" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> List(int pageNumber = 1, int pageSize = 10, string? status = null, string? priority = null, string? searchText = null)
        {
            try
            {
                var result = await _service.GetApplicationsListAsync(pageNumber, pageSize, status, null, priority, searchText);
                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting gattur complaints list");
                return Json(new { success = false, message = "Error retrieving applications list" });
            }
        }
    }
}
