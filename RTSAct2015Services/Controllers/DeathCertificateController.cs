using Microsoft.AspNetCore.Mvc;
using RTSAct2015.Models.DTOs;
using RTSAct2015.Services;

namespace RTSAct2015.Controllers
{
    public class DeathCertificateController : Controller
    {
        private readonly IDeathCertificateService _deathCertificateService;
        private readonly ILogger<DeathCertificateController> _logger;

        public DeathCertificateController(
            IDeathCertificateService deathCertificateService,
            ILogger<DeathCertificateController> logger)
        {
            _deathCertificateService = deathCertificateService ?? throw new ArgumentNullException(nameof(deathCertificateService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DeathCertificateCreateDto dto)
        {
            try
            {
                if (dto == null)
                {
                    _logger.LogWarning("Death certificate application received with null data");
                    return Json(new { success = false, message = "अवैध डेटा प्राप्त झाला." });
                }

                if (!ModelState.IsValid)
                {
                    var errors = ModelState
                        .Where(x => x.Value?.Errors.Count > 0)
                        .Select(x => $"{x.Key}: {string.Join(", ", x.Value!.Errors.Select(e => e.ErrorMessage))}")
                        .ToList();

                    _logger.LogWarning("Death certificate application validation failed: {Errors}", string.Join("; ", errors));
                    return Json(new { success = false, message = "कृपया सर्व आवश्यक फील्ड योग्यरित्या भरा." });
                }

                var result = await _deathCertificateService.CreateDeathCertificateAsync(dto);

                if (result.Success)
                {
                    _logger.LogInformation("Death certificate application created successfully with ID: {ApplicationId}", result.ApplicationId);
                    return Json(new
                    {
                        success = true,
                        message = result.Message,
                        data = new { applicationId = result.ApplicationId }
                    });
                }

                _logger.LogWarning("Death certificate application failed: {Message}", result.Message);
                return Json(new { success = false, message = result.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while processing death certificate application");
                return Json(new
                {
                    success = false,
                    message = "अर्ज प्रक्रिया करताना अनपेक्षित त्रुटी आली. कृपया पुन्हा प्रयत्न करा."
                });
            }
        }
    }
}
