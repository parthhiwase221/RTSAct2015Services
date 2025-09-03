using RTSAct2015Services.Interfaces.IRepository;
using RTSAct2015Services.Interfaces.IServices;
using RTSAct2015Services.Models.DTOs;
using RTSAct2015Services.Models.Entities;
using Microsoft.Data.SqlClient; // ✅ Correct using statement
using System.Data;

namespace RTSAct2015Services.Services
{
    public class OFCPermissionService : IOFCPermissionService
    {
        private readonly IOFCPermissionRepository _repository;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<OFCPermissionService> _logger;

        public OFCPermissionService(
            IOFCPermissionRepository repository,
            IWebHostEnvironment environment,
            ILogger<OFCPermissionService> logger)
        {
            _repository = repository;
            _environment = environment;
            _logger = logger;
        }

        public async Task<ResponseDto<ApplicationResponseDto>> CreateApplicationAsync(OFCPermissionCreateDto createDto)
        {
            try
            {
                _logger.LogInformation("Starting OFC application creation process...");

                // Process file upload BEFORE database operation
                if (createDto.DocumentFile != null)
                {
                    try
                    {
                        createDto.DocumentPath = await SaveDocumentAsync(createDto.DocumentFile);
                        createDto.DocumentName = createDto.DocumentFile.FileName;
                        createDto.DocumentType = Path.GetExtension(createDto.DocumentFile.FileName);
                        createDto.DocumentSize = createDto.DocumentFile.Length;

                        _logger.LogInformation("File uploaded successfully: {Path}", createDto.DocumentPath);
                    }
                    catch (Exception fileEx)
                    {
                        _logger.LogError(fileEx, "File upload failed: {Message}", fileEx.Message);
                        return new ResponseDto<ApplicationResponseDto>
                        {
                            Success = false,
                            Message = "File upload failed: " + fileEx.Message,
                            ErrorCode = "FILE_UPLOAD_ERROR"
                        };
                    }
                }

                // Call repository with proper exception handling
                string applicationId;
                try
                {
                    applicationId = await _repository.InsertApplicationAsync(createDto);
                    _logger.LogInformation("Repository insert completed with ApplicationID: {ApplicationId}", applicationId);
                }
                catch (SqlException sqlEx) // ✅ Now this will work
                {
                    _logger.LogError(sqlEx, "SQL Error during insert - Number: {Number}, Message: {Message}, Procedure: {Procedure}",
                        sqlEx.Number, sqlEx.Message, sqlEx.Procedure);

                    return new ResponseDto<ApplicationResponseDto>
                    {
                        Success = false,
                        Message = $"Database error: {sqlEx.Message}",
                        ErrorCode = "SQL_ERROR_" + sqlEx.Number
                    };
                }
                catch (Exception dbEx)
                {
                    _logger.LogError(dbEx, "Database operation failed: {Message}", dbEx.Message);

                    return new ResponseDto<ApplicationResponseDto>
                    {
                        Success = false,
                        Message = "Database operation failed: " + dbEx.Message,
                        ErrorCode = "DB_OPERATION_ERROR"
                    };
                }

                _logger.LogInformation("OFC permission application created successfully: {ApplicationId}", applicationId);

                return new ResponseDto<ApplicationResponseDto>
                {
                    Success = true,
                    Message = "OFC permission application submitted successfully",
                    Data = new ApplicationResponseDto
                    {
                        Status = "SUCCESS",
                        ApplicationID = applicationId,
                        Message = "Your OFC installation permission application has been submitted successfully"
                    }
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in CreateApplicationAsync: {Message}\nStackTrace: {StackTrace}",
                    ex.Message, ex.StackTrace);

                return new ResponseDto<ApplicationResponseDto>
                {
                    Success = false,
                    Message = "Unexpected system error occurred. Please try again later.",
                    ErrorCode = "SYSTEM_ERROR"
                };
            }
        }

        public async Task<string> SaveDocumentAsync(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    throw new ArgumentException("File is empty or null");
                }

                // File size validation (10MB limit)
                const int maxSizeBytes = 10 * 1024 * 1024;
                if (file.Length > maxSizeBytes)
                {
                    throw new ArgumentException($"File size ({file.Length} bytes) exceeds maximum allowed size (10MB)");
                }

                // File type validation
                var allowedExtensions = new[] { ".pdf", ".doc", ".docx", ".jpg", ".jpeg", ".png", ".bmp" };
                var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    throw new ArgumentException($"File type '{fileExtension}' is not allowed");
                }

                // Create directory structure using your existing folder structure
                var webRoot = _environment.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                var uploadsPath = Path.Combine(webRoot, "uploads", "ofc-permission", "general");
                Directory.CreateDirectory(uploadsPath);

                // Generate unique filename
                var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
                var fullFilePath = Path.Combine(uploadsPath, uniqueFileName);

                // Save file to disk
                using (var stream = new FileStream(fullFilePath, FileMode.Create, FileAccess.Write))
                {
                    await file.CopyToAsync(stream);
                }

                var relativePath = $"/uploads/ofc-permission/general/{uniqueFileName}";
                _logger.LogInformation("File saved successfully: {Path}", relativePath);

                return relativePath;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving file: {FileName}", file?.FileName ?? "NULL");
                throw;
            }
        }

        // Other methods remain the same...
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
                _logger.LogError(ex, "Error getting OFC permission application by ID: {ApplicationId}", applicationId);
                return new ResponseDto<ApplicationEntity>
                {
                    Success = false,
                    Message = "Error retrieving application"
                };
            }
        }

        public async Task<ResponseDto<IEnumerable<ApplicationEntity>>> GetApplicationsListAsync(
            int pageNumber = 1, int pageSize = 10, string? status = null, string? priority = null, string? searchText = null)
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
                _logger.LogError(ex, "Error getting OFC permission applications list");
                return new ResponseDto<IEnumerable<ApplicationEntity>>
                {
                    Success = false,
                    Message = "Error retrieving applications"
                };
            }
        }

        public async Task<ResponseDto<bool>> UpdateApplicationAsync(string applicationId, OFCPermissionCreateDto updateDto)
        {
            try
            {
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
                _logger.LogError(ex, "Error updating OFC permission application: {ApplicationId}", applicationId);
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
                _logger.LogError(ex, "Error deleting OFC permission application: {ApplicationId}", applicationId);
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
            return $"OFC{now:yyyyMMdd}-{now:HHmmss}-{Random.Shared.Next(100, 999)}";
        }
    }
}
