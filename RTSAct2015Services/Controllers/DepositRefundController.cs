using Microsoft.AspNetCore.Mvc;
using RTSAct2015Services.Interfaces.IServices;
using RTSAct2015Services.Models.DTOs;

namespace RTSAct2015Services.Controllers
{
    public class DepositRefundController : Controller
    {
        private readonly IDepositRefundService _service;
        private readonly ILogger<DepositRefundController> _logger;

        public DepositRefundController(IDepositRefundService service, ILogger<DepositRefundController> logger)
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
        public async Task<IActionResult> Create([FromForm] DepositRefundCreateDto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = new List<string>();

                    foreach (var modelError in ModelState)
                    {
                        foreach (var error in modelError.Value.Errors)
                        {
                            var fieldName = modelError.Key.Replace("model.", "");
                            errors.Add($"{fieldName}: {error.ErrorMessage}");
                        }
                    }

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
                    return Json(new
                    {
                        success = true,
                        message = "सिक्युरिटी डिपॉझिट परतावा अर्ज यशस्वीरित्या सबमिट झाला!",
                        data = new
                        {
                            applicationId = result.Data.ApplicationID,
                            status = result.Data.Status,
                            message = result.Data.Message
                        }
                    });
                }

                return Json(new
                {
                    success = false,
                    message = result.Message ?? "अर्ज सबमिट करण्यात अपयश. कृपया पुन्हा प्रयत्न करा.",
                    errors = new[] { result.Message ?? "Service error" }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in deposit refund controller");
                return Json(new
                {
                    success = false,
                    message = "अनपेक्षित त्रुटी आली. कृपया पुन्हा प्रयत्न करा.",
                    errors = new[] { "Server error" }
                });
            }
        }
    }
}