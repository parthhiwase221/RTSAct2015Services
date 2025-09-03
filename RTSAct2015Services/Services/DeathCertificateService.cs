using RTSAct2015.Models.DTOs;
using RTSAct2015.Data.Interfaces;

namespace RTSAct2015.Services
{
    public class DeathCertificateService : IDeathCertificateService
    {
        private readonly IDeathCertificateRepository _repository;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<DeathCertificateService> _logger;

        public DeathCertificateService(
            IDeathCertificateRepository repository,
            IWebHostEnvironment environment,
            ILogger<DeathCertificateService> logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<(bool Success, string ApplicationId, string Message)> CreateDeathCertificateAsync(DeathCertificateCreateDto dto)
        {
            if (dto == null)
            {
                return (false, string.Empty, "Invalid application data");
            }

            try
            {
                // Handle file uploads
                var filePaths = new Dictionary<string, string>();

                if (dto.MedicalCertificate != null && dto.MedicalCertificate.Length > 0)
                {
                    var medicalPath = await SaveFileAsync(dto.MedicalCertificate, "death-certificates/medical");
                    if (!string.IsNullOrEmpty(medicalPath))
                    {
                        filePaths["MedicalCertificate"] = medicalPath;
                    }
                }

                if (dto.IdProofDocument != null && dto.IdProofDocument.Length > 0)
                {
                    var idProofPath = await SaveFileAsync(dto.IdProofDocument, "death-certificates/id-proofs");
                    if (!string.IsNullOrEmpty(idProofPath))
                    {
                        filePaths["IdProofDocument"] = idProofPath;
                    }
                }

                if (dto.AddressProofDocument != null && dto.AddressProofDocument.Length > 0)
                {
                    var addressProofPath = await SaveFileAsync(dto.AddressProofDocument, "death-certificates/address-proofs");
                    if (!string.IsNullOrEmpty(addressProofPath))
                    {
                        filePaths["AddressProofDocument"] = addressProofPath;
                    }
                }

                if (dto.AdditionalDocument != null && dto.AdditionalDocument.Length > 0)
                {
                    var additionalPath = await SaveFileAsync(dto.AdditionalDocument, "death-certificates/additional");
                    if (!string.IsNullOrEmpty(additionalPath))
                    {
                        filePaths["AdditionalDocument"] = additionalPath;
                    }
                }

                return await _repository.CreateDeathCertificateAsync(dto, filePaths);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing death certificate application");
                return (false, string.Empty, $"Error processing application: {ex.Message}");
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
