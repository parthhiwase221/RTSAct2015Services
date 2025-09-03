using Microsoft.AspNetCore.Mvc;
using RTSAct2015.Models.DTOs;
using RTSAct2015.Services;

namespace RTSAct2015.Controllers
{
    public class BirthCertificateController : Controller
    {
        private readonly IBirthCertificateService _birthCertificateService;
        private readonly ILogger<BirthCertificateController> _logger;

        public BirthCertificateController(
            IBirthCertificateService birthCertificateService,
            ILogger<BirthCertificateController> logger)
        {
            _birthCertificateService = birthCertificateService ?? throw new ArgumentNullException(nameof(birthCertificateService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BirthCertificateCreateDto dto)
        {
            try
            {
                if (dto == null)
                {
                    _logger.LogWarning("Birth certificate application received with null data");
                    return Json(new { success = false, message = "अवैध डेटा प्राप्त झाला." });
                }

                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(x => x.Value?.Errors.Count > 0)
                        .Select(x => $"{x.Key}: {string.Join(", ", x.Value!.Errors.Select(e => e.ErrorMessage))}")
                        .ToList();

                    _logger.LogWarning("Birth certificate application validation failed: {Errors}", string.Join("; ", errors));
                    return Json(new { success = false, message = "कृपया सर्व आवश्यक फील्ड योग्यरित्या भरा." });
                }

                var result = await _birthCertificateService.CreateBirthCertificateAsync(dto);

                if (result.Success)
                {
                    _logger.LogInformation("Birth certificate application created successfully with ID: {ApplicationId}", result.ApplicationId);
                    return Json(new
                    {
                        success = true,
                        message = result.Message,
                        data = new { applicationId = result.ApplicationId }
                    });
                }

                _logger.LogWarning("Birth certificate application failed: {Message}", result.Message);
                return Json(new { success = false, message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while processing birth certificate application");
                return Json(new
                {
                    success = false,
                    message = "अर्ज प्रक्रिया करताना अनपेक्षित त्रुटी आली. कृपया पुन्हा प्रयत्न करा."
                });
            }
        }
    }
}
