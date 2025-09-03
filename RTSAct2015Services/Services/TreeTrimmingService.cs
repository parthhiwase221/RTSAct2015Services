using RTSAct2015Services.Interfaces.IRepository;
using RTSAct2015Services.Interfaces.IServices;
using RTSAct2015Services.Models.DTOs;
using RTSAct2015Services.Models.Entities;

namespace RTSAct2015Services.Services
{
    public class TreeTrimmingService : ITreeTrimmingService
    {
        private readonly ITreeTrimmingRepository _repository;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<TreeTrimmingService> _logger;

        public TreeTrimmingService(
            ITreeTrimmingRepository repository,
            IWebHostEnvironment environment,
            ILogger<TreeTrimmingService> logger)
        {
            _repository = repository;
            _environment = environment;
            _logger = logger;
        }

        public async Task<ResponseDto<ApplicationResponseDto>> CreateApplicationAsync(TreeTrimmingCreateDto createDto)
        {
            try
            {
                // **STEP 1: Process ALL file uploads BEFORE inserting to database**
                await ProcessAllFileUploadsAsync(createDto);

                // **STEP 2: Insert application data with file paths into database**
                var applicationId = await _repository.InsertApplicationAsync(createDto);

                _logger.LogInformation("Tree trimming application created: {ApplicationId} with {FileCount} files",
                    applicationId, CountUploadedFiles(createDto));

                return new ResponseDto<ApplicationResponseDto>
                {
                    Success = true,
                    Message = "Tree trimming application submitted successfully",
                    Data = new ApplicationResponseDto
                    {
                        Status = "SUCCESS",
                        ApplicationID = applicationId,
                        Message = "Your tree trimming application has been submitted successfully"
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating tree trimming application");
                return new ResponseDto<ApplicationResponseDto>
                {
                    Success = false,
                    Message = "Error submitting application. Please try again.",
                    ErrorCode = "TTR_CREATE_ERROR"
                };
            }
        }

        /// <summary>
        /// **NEW METHOD: Process ALL file uploads and set file paths in DTO**
        /// </summary>
        private async Task ProcessAllFileUploadsAsync(TreeTrimmingCreateDto dto)
        {
            try
            {
                _logger.LogInformation("Starting tree trimming file upload processing...");

                // **1. Main Document File**
                if (dto.DocumentFile != null)
                {
                    dto.DocumentPath = await SaveDocumentAsync(dto.DocumentFile, "general");
                    dto.DocumentName = dto.DocumentFile.FileName;
                    dto.DocumentType = Path.GetExtension(dto.DocumentFile.FileName);
                    dto.DocumentSize = dto.DocumentFile.Length;
                }

                // **2. Property Tax Receipt File**
                if (dto.PropertyTaxReceiptFile != null)
                {
                    dto.PropertyTaxReceiptFilePath = await SaveDocumentAsync(dto.PropertyTaxReceiptFile, "property-tax-receipts");
                }

                // **3. Tree Photograph File**
                if (dto.TreePhotographFile != null)
                {
                    dto.TreePhotographFilePath = await SaveDocumentAsync(dto.TreePhotographFile, "tree-photographs");
                }

                // **4. Aadhaar Card File**
                if (dto.AadhaarCardFile != null)
                {
                    dto.AadhaarCardFilePath = await SaveDocumentAsync(dto.AadhaarCardFile, "aadhaar-cards");
                }

                // **5. Building Permission File**
                if (dto.BuildingPermissionFile != null)
                {
                    dto.BuildingPermissionFilePath = await SaveDocumentAsync(dto.BuildingPermissionFile, "building-permissions");
                }

                // **6. Sanctioned Plan File**
                if (dto.SanctionedPlanFile != null)
                {
                    dto.SanctionedPlanFilePath = await SaveDocumentAsync(dto.SanctionedPlanFile, "sanctioned-plans");
                }

                // **7. NOC Letter File**
                if (dto.NOCLetterFile != null)
                {
                    dto.NOCLetterFilePath = await SaveDocumentAsync(dto.NOCLetterFile, "noc-letters");
                }

                var uploadedCount = CountUploadedFiles(dto);
                _logger.LogInformation("Tree trimming file upload processing completed. Total files processed: {Count}", uploadedCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during tree trimming file upload processing");
                throw;
            }
        }

        public async Task<string> SaveDocumentAsync(IFormFile file, string subfolder)
        {
            try
            {
                // File validation
                if (file == null || file.Length == 0)
                {
                    throw new ArgumentException("File is empty or null");
                }

                // File size validation (5MB limit)
                const int maxSizeBytes = 5 * 1024 * 1024; // 5MB
                if (file.Length > maxSizeBytes)
                {
                    throw new ArgumentException($"File size ({file.Length} bytes) exceeds maximum allowed size ({maxSizeBytes} bytes)");
                }

                // File type validation
                var allowedExtensions = new[] { ".pdf", ".jpg", ".jpeg", ".png", ".bmp" };
                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    throw new ArgumentException($"File type '{fileExtension}' is not allowed. Allowed types: {string.Join(", ", allowedExtensions)}");
                }

                // Create directory structure
                var webRoot = _environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                var uploadsPath = Path.Combine(webRoot, "uploads", "tree-trimming", subfolder);
                Directory.CreateDirectory(uploadsPath);

                // Generate unique filename
                var originalFileName = Path.GetFileNameWithoutExtension(file.FileName);
                var sanitizedFileName = string.Join("", originalFileName.Take(50));
                var uniqueFileName = $"{sanitizedFileName}_{Guid.NewGuid()}{fileExtension}";
                var fullFilePath = Path.Combine(uploadsPath, uniqueFileName);

                // Save file to disk
                using (var stream = new FileStream(fullFilePath, FileMode.Create, FileAccess.Write))
                {
                    await file.CopyToAsync(stream);
                }

                // Return relative path for database storage
                var relativePath = $"/uploads/tree-trimming/{subfolder}/{uniqueFileName}";

                _logger.LogInformation("Tree trimming file saved successfully: Original='{OriginalName}' Saved='{SavedPath}' Size={Size} bytes",
                    file.FileName, relativePath, file.Length);

                return relativePath;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving tree trimming document: FileName='{FileName}' Subfolder='{Subfolder}'",
                    file?.FileName ?? "NULL", subfolder);
                throw;
            }
        }

        /// <summary>
        /// **UTILITY METHOD: Count uploaded files for logging**
        /// </summary>
        private int CountUploadedFiles(TreeTrimmingCreateDto dto)
        {
            var count = 0;
            if (dto.DocumentFile != null) count++;
            if (dto.PropertyTaxReceiptFile != null) count++;
            if (dto.TreePhotographFile != null) count++;
            if (dto.AadhaarCardFile != null) count++;
            if (dto.BuildingPermissionFile != null) count++;
            if (dto.SanctionedPlanFile != null) count++;
            if (dto.NOCLetterFile != null) count++;
            return count;
        }

        // **EXISTING METHODS (updated to use ApplicationEntity)**

        public async Task<ResponseDto<ApplicationEntity>> GetApplicationByIdAsync(string applicationId)
        {
            try
            {
                var application = await _repository.GetApplicationByIdAsync(applicationId);

                if (application == null)
                {
                    return new ResponseDto<ApplicationEntity>
                    {
                        Success = false,
                        Message = "Application not found"
                    };
                }

                return new ResponseDto<ApplicationEntity>
                {
                    Success = true,
                    Data = application,
                    Message = "Application found"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting tree trimming application by ID: {ApplicationId}", applicationId);
                return new ResponseDto<ApplicationEntity>
                {
                    Success = false,
                    Message = "Error retrieving application"
                };
            }
        }

        public async Task<ResponseDto<IEnumerable<ApplicationEntity>>> GetApplicationsListAsync(
            int pageNumber = 1,
            int pageSize = 10,
            string? status = null,
            string? priority = null,
            string? searchText = null)
        {
            try
            {
                var applications = await _repository.GetApplicationsListAsync(pageNumber, pageSize, status, priority, searchText);

                return new ResponseDto<IEnumerable<ApplicationEntity>>
                {
                    Success = true,
                    Data = applications,
                    Message = "Applications retrieved successfully"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting tree trimming applications list");
                return new ResponseDto<IEnumerable<ApplicationEntity>>
                {
                    Success = false,
                    Message = "Error retrieving applications"
                };
            }
        }

        public async Task<ResponseDto<bool>> UpdateApplicationAsync(string applicationId, TreeTrimmingCreateDto updateDto)
        {
            try
            {
                // Process file uploads for updates
                await ProcessAllFileUploadsAsync(updateDto);

                var result = await _repository.UpdateApplicationAsync(applicationId, updateDto);

                return new ResponseDto<bool>
                {
                    Success = result,
                    Data = result,
                    Message = result ? "Application updated successfully" : "Failed to update application"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating tree trimming application: {ApplicationId}", applicationId);
                return new ResponseDto<bool>
                {
                    Success = false,
                    Message = "Error updating application"
                };
            }
        }

        public async Task<ResponseDto<bool>> DeleteApplicationAsync(string applicationId, string deletedBy)
        {
            try
            {
                var result = await _repository.DeleteApplicationAsync(applicationId, deletedBy);

                return new ResponseDto<bool>
                {
                    Success = result,
                    Data = result,
                    Message = result ? "Application deleted successfully" : "Failed to delete application"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting tree trimming application: {ApplicationId}", applicationId);
                return new ResponseDto<bool>
                {
                    Success = false,
                    Message = "Error deleting application"
                };
            }
        }

        public string GenerateApplicationId()
        {
            var now = DateTime.Now;
            return $"TTR{now:yyyyMMdd}-{now:HHmmss}-{Random.Shared.Next(100, 999)}";
        }
    }
}
