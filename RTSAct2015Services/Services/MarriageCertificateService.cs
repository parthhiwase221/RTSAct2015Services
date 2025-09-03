using RTSAct2015.Models.DTOs;
using RTSAct2015.Data.Interfaces;

namespace RTSAct2015.Services
{
    public class MarriageCertificateService : IMarriageCertificateService
    {
        private readonly IMarriageCertificateRepository _repository;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<MarriageCertificateService> _logger;

        public MarriageCertificateService(
            IMarriageCertificateRepository repository,
            IWebHostEnvironment environment,
            ILogger<MarriageCertificateService> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<(bool Success, string ApplicationId, string Message)> CreateMarriageCertificateAsync(MarriageCertificateCreateDto dto)
        {
            if (dto == null)
            {
                return (false, string.Empty, "Invalid application data");
            }

            try
            {
                // Handle file uploads
                var filePaths = new Dictionary<string, string>();

                if (dto.GroomPhoto != null && dto.GroomPhoto.Length > 0)
                {
                    var groomPhotoPath = await SaveFileAsync(dto.GroomPhoto, "marriage-certificates/groom-photos");
                    if (!string.IsNullOrEmpty(groomPhotoPath))
                    {
                        filePaths["GroomPhoto"] = groomPhotoPath;
                    }
                }

                if (dto.BridePhoto != null && dto.BridePhoto.Length > 0)
                {
                    var bridePhotoPath = await SaveFileAsync(dto.BridePhoto, "marriage-certificates/bride-photos");
                    if (!string.IsNullOrEmpty(bridePhotoPath))
                    {
                        filePaths["BridePhoto"] = bridePhotoPath;
                    }
                }

                if (dto.PriestPhoto != null && dto.PriestPhoto.Length > 0)
                {
                    var priestPhotoPath = await SaveFileAsync(dto.PriestPhoto, "marriage-certificates/priest-photos");
                    if (!string.IsNullOrEmpty(priestPhotoPath))
                    {
                        filePaths["PriestPhoto"] = priestPhotoPath;
                    }
                }

                if (dto.MarriageCard != null && dto.MarriageCard.Length > 0)
                {
                    var marriageCardPath = await SaveFileAsync(dto.MarriageCard, "marriage-certificates/marriage-cards");
                    if (!string.IsNullOrEmpty(marriageCardPath))
                    {
                        filePaths["MarriageCard"] = marriageCardPath;
                    }
                }

                if (dto.GroupPhoto != null && dto.GroupPhoto.Length > 0)
                {
                    var groupPhotoPath = await SaveFileAsync(dto.GroupPhoto, "marriage-certificates/group-photos");
                    if (!string.IsNullOrEmpty(groupPhotoPath))
                    {
                        filePaths["GroupPhoto"] = groupPhotoPath;
                    }
                }

                return await _repository.CreateMarriageCertificateAsync(dto, filePaths);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing marriage certificate application");
                return (false, string.Empty, ex.Message ?? "Error processing application"); // ✅ Fixed nullable warning
            }
        }

        private async Task<string> SaveFileAsync(IFormFile file, string folder)
        {
            if (file == null || file.Length == 0)
                return string.Empty;

            try
            {
                // Validate file size (10MB limit)
                const long maxFileSize = 10 * 1024 * 1024;
                if (file.Length > maxFileSize)
                {
                    throw new InvalidOperationException("File size exceeds 10MB limit");
                }

                // Validate file extension
                var allowedExtensions = new[] { ".pdf", ".jpg", ".jpeg", ".png" };
                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    throw new InvalidOperationException("Invalid file type. Only PDF, JPG, JPEG, and PNG files are allowed");
                }

                var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", folder);
                Directory.CreateDirectory(uploadsFolder);

                var safeFileName = Path.GetFileNameWithoutExtension(file.FileName);
                var uniqueFileName = $"{safeFileName}_{Guid.NewGuid():N}{fileExtension}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                return $"/uploads/{folder}/{uniqueFileName}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save file: {FileName}", file.FileName);
                throw new InvalidOperationException($"Failed to save file: {ex.Message}", ex);
            }
        }
    }
}
