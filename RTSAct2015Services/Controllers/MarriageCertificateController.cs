using Microsoft.AspNetCore.Mvc;
using RTSAct2015.Models.DTOs;
using RTSAct2015.Services;

namespace RTSAct2015.Controllers
{
    public class MarriageCertificateController : Controller
    {
        private readonly IMarriageCertificateService _marriageCertificateService;
        private readonly ILogger<MarriageCertificateController> _logger;

        public MarriageCertificateController(
            IMarriageCertificateService marriageCertificateService,
            ILogger<MarriageCertificateController> logger)
        {
            _marriageCertificateService = marriageCertificateService ?? throw new ArgumentNullException(nameof(marriageCertificateService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MarriageCertificateCreateDto dto)
        {
            try
            {
                if (dto == null)
                {
                    _logger.LogWarning("Marriage certificate application received with null data");
                    return Json(new { success = false, message = "अवैध डेटा प्राप्त झाला." });
                }

                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(x => x.Value?.Errors.Count > 0)
                        .Select(x => $"{x.Key}: {string.Join(", ", x.Value!.Errors.Select(e => e.ErrorMessage))}")
                        .ToList();

                    _logger.LogWarning("Marriage certificate application validation failed: {Errors}", string.Join("; ", errors));
                    return Json(new { success = false, message = "कृपया सर्व आवश्यक फील्ड योग्यरित्या भरा." });
                }

                // ✅ Add detailed logging here
                _logger.LogInformation("Starting marriage certificate creation for: {GroomName} & {BrideName}", dto.GroomName, dto.BrideName);

                var result = await _marriageCertificateService.CreateMarriageCertificateAsync(dto);

                if (result.Success)
                {
                    _logger.LogInformation("Marriage certificate application created successfully with ID: {ApplicationId}", result.ApplicationId);
                    return Json(new
                    {
                        success = true,
                        message = result.Message ?? "Application submitted successfully",
                        data = new { applicationId = result.ApplicationId ?? "N/A" }
                    });
                }

                // ✅ Log the exact failure reason
                _logger.LogError("Marriage certificate application failed: {Message}", result.Message);
                return Json(new { success = false, message = $"Debug Error: {result.Message}" }); // Temporary debug message
            }
            catch (Exception ex)
            {
                // ✅ Log the complete exception details
                _logger.LogError(ex, "Unexpected error occurred while processing marriage certificate application: {Message}", ex.Message);
                return Json(new
                {
                    success = false,
                    message = $"Exception: {ex.Message} | InnerException: {ex.InnerException?.Message}" // Temporary debug
                });
            }
        }

    }
}
