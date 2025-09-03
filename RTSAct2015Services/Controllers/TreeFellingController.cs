using Microsoft.AspNetCore.Mvc;
using RTSAct2015Services.Interfaces.IServices;
using RTSAct2015Services.Models.DTOs;

namespace RTSAct2015Services.Controllers
{
    public class TreeFellingController : Controller
    {
        private readonly ITreeFellingService _service;
        private readonly ILogger<TreeFellingController> _logger;

        public TreeFellingController(ITreeFellingService service, ILogger<TreeFellingController> logger)
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
        public async Task<IActionResult> Create([FromForm] TreeFellingCreateDto model)
        {
            try
            {
                // **CUSTOM CHECKBOX VALIDATION - NO ATTRIBUTES NEEDED**
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

                    _logger.LogWarning("Tree Felling validation failed: {Errors}", string.Join(", ", errors));

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
                    _logger.LogInformation("Tree felling application created: {ApplicationId}", result.Data.ApplicationID);

                    return Json(new
                    {
                        success = true,
                        message = "वृक्ष तोड अर्ज यशस्वीरित्या सबमिट झाला!",
                        data = new
                        {
                            applicationId = result.Data.ApplicationID,
                            status = result.Data.Status,
                            message = result.Data.Message,
                            treesToPlant = model.NoOfTreeFelling * 10 // 1:10 ratio
                        }
                    });
                }

                _logger.LogError("Tree Felling service returned failure: {Message}", result.Message);
                return Json(new
                {
                    success = false,
                    message = result.Message ?? "अर्ज सबमिट करण्यात अपयश. कृपया पुन्हा प्रयत्न करा.",
                    errors = new[] { result.Message ?? "Service error" }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in tree felling controller");
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
                _logger.LogError(ex, "Error getting tree felling application by ID: {Id}", id);
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
                _logger.LogError(ex, "Error getting tree felling applications list");
                return Json(new { success = false, message = "Error retrieving applications list" });
            }
        }
    }
}
