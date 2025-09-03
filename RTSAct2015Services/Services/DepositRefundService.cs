using RTSAct2015Services.Interfaces.IRepository;
using RTSAct2015Services.Interfaces.IServices;
using RTSAct2015Services.Models.DTOs;
using RTSAct2015Services.Models.Entities;

namespace RTSAct2015Services.Services
{
    public class DepositRefundService : IDepositRefundService
    {
        private readonly IDepositRefundRepository _repository;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<DepositRefundService> _logger;

        public DepositRefundService(
            IDepositRefundRepository repository,
            IWebHostEnvironment environment,
            ILogger<DepositRefundService> logger)
        {
            _repository = repository;
            _environment = environment;
            _logger = logger;
        }

        public async Task<ResponseDto<ApplicationResponseDto>> CreateApplicationAsync(DepositRefundCreateDto createDto)
        {
            try
            {
                // **STEP 1: Process ALL file uploads BEFORE inserting to database**
                await ProcessAllFileUploadsAsync(createDto);

                // **STEP 2: Insert application data with file paths into database**
                var applicationId = await _repository.InsertApplicationAsync(createDto);

                _logger.LogInformation("Deposit refund application created: {ApplicationId} with {FileCount} files",
                    applicationId, CountUploadedFiles(createDto));

                return new ResponseDto<ApplicationResponseDto>
                {
                    Success = true,
                    Message = "Application submitted successfully",
                    Data = new ApplicationResponseDto
                    {
                        Status = "SUCCESS",
                        ApplicationID = applicationId,
                        Message = "Your deposit refund application has been submitted successfully"
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating deposit refund application");
                return new ResponseDto<ApplicationResponseDto>
                {
                    Success = false,
                    Message = "Error submitting application. Please try again.",
                    ErrorCode = "DEP_CREATE_ERROR"
                };
            }
        }

        /// <summary>
        /// **ENHANCED METHOD: Process ALL file uploads and set file paths in DTO**
        /// This method handles all document uploads for the deposit refund form
        /// </summary>
        private async Task ProcessAllFileUploadsAsync(DepositRefundCreateDto dto)
        {
            try
            {
                _logger.LogInformation("Starting file upload processing...");

                // **1. Main Document File**
                if (dto.DocumentFile != null)
                {
                    _logger.LogInformation("Processing main document file: {FileName}", dto.DocumentFile.FileName);
                    dto.DocumentPath = await SaveDocumentAsync(dto.DocumentFile, "general");
                    dto.DocumentName = dto.DocumentFile.FileName;
                    dto.DocumentType = Path.GetExtension(dto.DocumentFile.FileName);
                    dto.DocumentSize = dto.DocumentFile.Length;
                    _logger.LogInformation("Main document saved: {Path}", dto.DocumentPath);
                }

                // **2. Occupancy Certificate File**
                if (dto.OccupancyCertFile != null)
                {
                    _logger.LogInformation("Processing occupancy certificate file: {FileName}", dto.OccupancyCertFile.FileName);
                    dto.OccupancyCertFilePath = await SaveDocumentAsync(dto.OccupancyCertFile, "occupancy-certificates");
                    _logger.LogInformation("Occupancy certificate saved: {Path}", dto.OccupancyCertFilePath);
                }

                // **3. Building Deposit Receipt File**
                if (dto.BuildingDepReceiptFile != null)
                {
                    _logger.LogInformation("Processing building deposit receipt file: {FileName}", dto.BuildingDepReceiptFile.FileName);
                    dto.BuildingDepReceiptFilePath = await SaveDocumentAsync(dto.BuildingDepReceiptFile, "building-deposit-receipts");
                    _logger.LogInformation("Building deposit receipt saved: {Path}", dto.BuildingDepReceiptFilePath);
                }

                // **4. Tree Deposit Receipt File**
                if (dto.TreeDepReceiptFile != null)
                {
                    _logger.LogInformation("Processing tree deposit receipt file: {FileName}", dto.TreeDepReceiptFile.FileName);
                    dto.TreeDepReceiptFilePath = await SaveDocumentAsync(dto.TreeDepReceiptFile, "tree-deposit-receipts");
                    _logger.LogInformation("Tree deposit receipt saved: {Path}", dto.TreeDepReceiptFilePath);
                }

                // **5. Property Tax Receipt File**
                if (dto.PropertyTaxReceiptFile != null)
                {
                    _logger.LogInformation("Processing property tax receipt file: {FileName}", dto.PropertyTaxReceiptFile.FileName);
                    dto.PropertyTaxReceiptFilePath = await SaveDocumentAsync(dto.PropertyTaxReceiptFile, "property-tax-receipts");
                    _logger.LogInformation("Property tax receipt saved: {Path}", dto.PropertyTaxReceiptFilePath);
                }

                // **6. Building Permission Certificate File**
                if (dto.BuildingPermissionCertFile != null)
                {
                    _logger.LogInformation("Processing building permission certificate file: {FileName}", dto.BuildingPermissionCertFile.FileName);
                    dto.BuildingPermissionCertFilePath = await SaveDocumentAsync(dto.BuildingPermissionCertFile, "building-permission-certs");
                    _logger.LogInformation("Building permission certificate saved: {Path}", dto.BuildingPermissionCertFilePath);
                }

                // **7. Challan File**
                if (dto.ChallanFile != null)
                {
                    _logger.LogInformation("Processing challan file: {FileName}", dto.ChallanFile.FileName);
                    dto.ChallanFilePath = await SaveDocumentAsync(dto.ChallanFile, "challans");
                    _logger.LogInformation("Challan file saved: {Path}", dto.ChallanFilePath);
                }

                // **8. NOC File**
                if (dto.NOCFile != null)
                {
                    _logger.LogInformation("Processing NOC file: {FileName}", dto.NOCFile.FileName);
                    dto.NOCFilePath = await SaveDocumentAsync(dto.NOCFile, "noc-letters");
                    _logger.LogInformation("NOC file saved: {Path}", dto.NOCFilePath);
                }

                var uploadedCount = CountUploadedFiles(dto);
                _logger.LogInformation("File upload processing completed. Total files processed: {Count}", uploadedCount);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during file upload processing");
                throw;
            }
        }

        /// <summary>
        /// **ENHANCED METHOD: Save document with comprehensive error handling and validation**
        /// </summary>
        public async Task<string> SaveDocumentAsync(IFormFile file, string subfolder)
        {
            try
            {
                // **Validation**
                if (file == null || file.Length == 0)
                {
                    throw new ArgumentException("File is empty or null");
                }

                // **File size validation (5MB limit)**
                const int maxSizeBytes = 5 * 1024 * 1024; // 5MB
                if (file.Length > maxSizeBytes)
                {
                    throw new ArgumentException($"File size ({file.Length} bytes) exceeds maximum allowed size ({maxSizeBytes} bytes)");
                }

                // **File type validation**
                var allowedExtensions = new[] { ".pdf", ".jpg", ".jpeg", ".png", ".bmp" };
                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    throw new ArgumentException($"File type '{fileExtension}' is not allowed. Allowed types: {string.Join(", ", allowedExtensions)}");
                }

                // **Create directory structure**
                var webRoot = _environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                var uploadsPath = Path.Combine(webRoot, "uploads", "deposit-refund", subfolder);
                Directory.CreateDirectory(uploadsPath);

                // **Generate unique filename to avoid conflicts**
                var originalFileName = Path.GetFileNameWithoutExtension(file.FileName);
                var sanitizedFileName = string.Join("", originalFileName.Take(50)); // Limit filename length
                var uniqueFileName = $"{sanitizedFileName}_{Guid.NewGuid()}{fileExtension}";
                var fullFilePath = Path.Combine(uploadsPath, uniqueFileName);

                // **Save file to disk**
                using (var stream = new FileStream(fullFilePath, FileMode.Create, FileAccess.Write))
                {
                    await file.CopyToAsync(stream);
                }

                // **Return relative path for database storage**
                var relativePath = $"/uploads/deposit-refund/{subfolder}/{uniqueFileName}";

                _logger.LogInformation("File saved successfully: Original='{OriginalName}' Saved='{SavedPath}' Size={Size} bytes",
                    file.FileName, relativePath, file.Length);

                return relativePath;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving document: FileName='{FileName}' Subfolder='{Subfolder}'",
                    file?.FileName ?? "NULL", subfolder);
                throw;
            }
        }

        /// <summary>
        /// **UTILITY METHOD: Count uploaded files for logging**
        /// </summary>
        private int CountUploadedFiles(DepositRefundCreateDto dto)
        {
            var count = 0;
            if (dto.DocumentFile != null) count++;
            if (dto.OccupancyCertFile != null) count++;
            if (dto.BuildingDepReceiptFile != null) count++;
            if (dto.TreeDepReceiptFile != null) count++;
            if (dto.PropertyTaxReceiptFile != null) count++;
            if (dto.BuildingPermissionCertFile != null) count++;
            if (dto.ChallanFile != null) count++;
            if (dto.NOCFile != null) count++;
            return count;
        }

        /// <summary>
        /// **UTILITY METHOD: Create upload directories if they don't exist**
        /// </summary>
        private void EnsureUploadDirectoriesExist()
        {
            var webRoot = _environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var baseUploadPath = Path.Combine(webRoot, "uploads", "deposit-refund");

            var subdirectories = new[]
            {
                "general",
                "occupancy-certificates",
                "building-deposit-receipts",
                "tree-deposit-receipts",
                "property-tax-receipts",
                "building-permission-certs",
                "challans",
                "noc-letters"
            };

            foreach (var subdir in subdirectories)
            {
                var dirPath = Path.Combine(baseUploadPath, subdir);
                Directory.CreateDirectory(dirPath);
            }

            _logger.LogInformation("Upload directories created/verified at: {Path}", baseUploadPath);
        }

        // **EXISTING METHODS (unchanged)**

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
                _logger.LogError(ex, "Error getting application by ID: {ApplicationId}", applicationId);
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
                _logger.LogError(ex, "Error getting applications list");
                return new ResponseDto<IEnumerable<ApplicationEntity>>
                {
                    Success = false,
                    Message = "Error retrieving applications"
                };
            }
        }

        public async Task<ResponseDto<bool>> UpdateApplicationAsync(string applicationId, DepositRefundCreateDto updateDto)
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
                _logger.LogError(ex, "Error updating application: {ApplicationId}", applicationId);
                return new ResponseDto<bool>
                {
                    Success = false,
                    Message = "Error updating application"
                };
            }
        }

        public async Task<ResponseDto<bool>> DeleteApplicationAsync(string applicationId)
        {
            try
            {
                var result = await _repository.DeleteApplicationAsync(applicationId);

                return new ResponseDto<bool>
                {
                    Success = result,
                    Data = result,
                    Message = result ? "Application deleted successfully" : "Failed to delete application"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting application: {ApplicationId}", applicationId);
                return new ResponseDto<bool>
                {
                    Success = false,
                    Message = "Error deleting application"
                };
            }
        }
    }
}
