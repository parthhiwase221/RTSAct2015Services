using Microsoft.AspNetCore.Mvc;
using RTSAct2015Services.Models.DTOs;
using RTSAct2015Services.Services.Interfaces;

namespace RTSAct2015Services.Controllers
{
    public class TrackController : Controller
    {
        private readonly ITrackApplicationService _trackService;
        private readonly ILogger<TrackController> _logger;

        public TrackController(ITrackApplicationService trackService, ILogger<TrackController> logger)
        {
            _trackService = trackService;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(new TrackApplicationDto());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(TrackApplicationDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var result = await _trackService.TrackApplicationAsync(model);

                if (result.IsFound)
                {
                    return View("Result", result);
                }
                else
                {
                    ModelState.AddModelError("", result.ErrorMessage);
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in TrackController");
                ModelState.AddModelError("", "तांत्रिक त्रुटी झाली. कृपया पुन्हा प्रयत्न करा / Technical error occurred. Please try again.");
                return View(model);
            }
        }

        [HttpGet]
        public IActionResult Result()
        {
            return RedirectToAction("Index");
        }
    }
}
