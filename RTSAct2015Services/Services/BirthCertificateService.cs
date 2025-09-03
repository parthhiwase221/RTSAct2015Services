using RTSAct2015.Models.DTOs;
using RTSAct2015.Data.Interfaces;

namespace RTSAct2015.Services
{
    public class BirthCertificateService : IBirthCertificateService
    {
        private readonly IBirthCertificateRepository _repository;
        private readonly IWebHostEnvironment _environment;

        public BirthCertificateService(IBirthCertificateRepository repository, IWebHostEnvironment environment)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
        }

        public async Task<(bool Success, string ApplicationId, string Message)> CreateBirthCertificateAsync(BirthCertificateCreateDto dto)
        {
            if (dto == null)
            {
                return (false, string.Empty, "Invalid application data");
            }

            try
            {
                // Handle file uploads
                var filePaths = new Dictionary<string, string>();

                if (dto.DischargeDocument != null && dto.DischargeDocument.Length > 0)
                {
                    var dischargePath = await SaveFileAsync(dto.DischargeDocument, "birth-certificates/discharge");
                    if (!string.IsNullOrEmpty(dischargePath))
                    {
                        filePaths["DischargeDocument"] = dischargePath;
                    }
                }

                if (dto.IdProofDocument != null && dto.IdProofDocument.Length > 0)
                {
                    var idProofPath = await SaveFileAsync(dto.IdProofDocument, "birth-certificates/id-proofs");
                    if (!string.IsNullOrEmpty(idProofPath))
                    {
                        filePaths["IdProofDocument"] = idProofPath;
                    }
                }

                if (dto.AddressProofDocument != null && dto.AddressProofDocument.Length > 0)
                {
                    var addressProofPath = await SaveFileAsync(dto.AddressProofDocument, "birth-certificates/address-proofs");
                    if (!string.IsNullOrEmpty(addressProofPath))
                    {
                        filePaths["AddressProofDocument"] = addressProofPath;
                    }
                }

                if (dto.AdditionalDocument != null && dto.AdditionalDocument.Length > 0)
                {
                    var additionalPath = await SaveFileAsync(dto.AdditionalDocument, "birth-certificates/additional");
                    if (!string.IsNullOrEmpty(additionalPath))
                    {
                        filePaths["AdditionalDocument"] = additionalPath;
                    }
                }

                return await _repository.CreateBirthCertificateAsync(dto, filePaths);
            }
            catch (Exception ex)
            {
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
                throw new InvalidOperationException($"Failed to save file: {ex.Message}", ex);
            }
        }
    }
}
