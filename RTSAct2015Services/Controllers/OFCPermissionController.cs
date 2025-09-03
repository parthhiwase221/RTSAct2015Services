using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting;
using RTSAct2015Services.Interfaces.IServices;
using RTSAct2015Services.Models.DTOs;

namespace RTSAct2015Services.Controllers
{
    public class OFCPermissionController : Controller
    {
        private readonly IOFCPermissionService _service;
        private readonly ILogger<OFCPermissionController> _logger;
        private readonly IWebHostEnvironment _env;

        public OFCPermissionController(
            IOFCPermissionService service,
            ILogger<OFCPermissionController> logger,
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
        public async Task<IActionResult> Create([FromForm] OFCPermissionCreateDto model)
        {
            try
            {
                _logger.LogInformation("=== OFC FORM SUBMISSION START ===");
                _logger.LogInformation("Form Data - Title: '{Title}', FirstName: '{FirstName}', LastName: '{LastName}', Mobile: '{Mobile}', Street: '{Street}', Area: '{Area}', City: '{City}', PinCode: '{PinCode}', Landmark: '{Landmark}', InstallationType: '{InstallationType}', CableType: '{CableType}', CompanyName: '{CompanyName}', TotalLength: {TotalLength}",
                    model.Title, model.FirstName, model.LastName, model.Mobile, model.Street, model.Area, model.City, model.PinCode, model.Landmark, model.InstallationType, model.CableType, model.CompanyName, model.TotalLength);

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

                    _logger.LogWarning("ModelState validation failed with {ErrorCount} errors", errors.Count);

                    return Json(new
                    {
                        success = false,
                        message = "कृपया खालील त्रुटी दुरुस्त करा:",
                        errors = errors
                    });
                }

                _logger.LogInformation("ModelState validation passed. Calling service...");

                var result = await _service.CreateApplicationAsync(model);

                _logger.LogInformation("Service call completed - Success: {Success}, Message: '{Message}', ErrorCode: '{ErrorCode}'",
                    result.Success, result.Message, result.ErrorCode);

                if (result.Success && result.Data != null)
                {
                    _logger.LogInformation("=== OFC FORM SUBMISSION SUCCESS ===");

                    return Json(new
                    {
                        success = true,
                        message = "OFC परवानगी अर्ज यशस्वीरित्या सबमिट झाला!",
                        data = new
                        {
                            applicationId = result.Data.ApplicationID,
                            status = result.Data.Status,
                            message = result.Data.Message
                        }
                    });
                }

                _logger.LogError("=== OFC FORM SUBMISSION FAILED ===");
                _logger.LogError("Service Error - Message: '{Message}', ErrorCode: '{ErrorCode}'", result.Message, result.ErrorCode);

                return Json(new
                {
                    success = false,
                    message = result.Message ?? "अर्ज सबमिट करण्यात अपयश. कृपया पुन्हा प्रयत्न करा.",
                    errors = new[] { result.Message ?? "Service error", result.ErrorCode ?? "UNKNOWN_ERROR" }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "=== CONTROLLER EXCEPTION ===");
                _logger.LogError(ex, "Exception Message: {Message}", ex.Message);
                _logger.LogError(ex, "Stack Trace: {StackTrace}", ex.StackTrace);

                return Json(new
                {
                    success = false,
                    message = "अनपेक्षित त्रुटी आली. कृपया पुन्हा प्रयत्न करा.",
                    errors = new[] { "Controller exception: " + ex.Message }
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
                _logger.LogError(ex, "Error getting OFC permission application by ID: {Id}", id);
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
                _logger.LogError(ex, "Error getting OFC permission applications list");
                return Json(new { success = false, message = "Error retrieving applications list" });
            }
        }
    }
}
