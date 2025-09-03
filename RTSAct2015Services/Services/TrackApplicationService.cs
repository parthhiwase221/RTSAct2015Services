using RTSAct2015Services.Interfaces.IRepository;
using RTSAct2015Services.Models.DTOs;
using RTSAct2015Services.Services.Interfaces;

namespace RTSAct2015Services.Services.Implementation
{
    public class TrackApplicationService : ITrackApplicationService
    {
        private readonly ITrackApplicationRepository _trackRepository;
        private readonly ILogger<TrackApplicationService> _logger;

        public TrackApplicationService(ITrackApplicationRepository trackRepository, ILogger<TrackApplicationService> logger)
        {
            _trackRepository = trackRepository;
            _logger = logger;
        }

        public async Task<TrackApplicationResponseDto> TrackApplicationAsync(TrackApplicationDto request)
        {
            try
            {
                _logger.LogInformation("Tracking application: {ComplaintNumber}", request.ComplaintNumber);

                var application = string.IsNullOrEmpty(request.MobileNumber)
                    ? await _trackRepository.GetApplicationByComplaintNumberAsync(request.ComplaintNumber)
                    : await _trackRepository.GetApplicationByComplaintNumberAndMobileAsync(request.ComplaintNumber, request.MobileNumber);

                if (application == null)
                {
                    return new TrackApplicationResponseDto
                    {
                        IsFound = false,
                        ErrorMessage = "तक्रार क्रमांक आढळला नाही. कृपया योग्य तक्रार क्रमांक प्रविष्ट करा / Application not found. Please enter correct complaint number."
                    };
                }

                return new TrackApplicationResponseDto
                {
                    IsFound = true,
                    ComplaintNumber = application.ApplicationID,
                    ApplicationType = application.ApplicationType,
                    FormName = application.FormName,
                    Status = application.Status,
                    Priority = application.Priority,
                    CreatedDate = application.CreatedDate,
                    UpdatedDate = application.UpdatedDate,
                    ResolvedDate = application.ResolvedDate,
                    ApplicantName = $"{application.Title} {application.FirstName} {application.MiddleName} {application.LastName}".Trim(),
                    Mobile = application.Mobile,
                    Email = application.Email,
                    Area = application.Area,
                    Remarks = application.Remarks,
                    AssignedTo = application.AssignedTo
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error tracking application: {ComplaintNumber}", request.ComplaintNumber);

                return new TrackApplicationResponseDto
                {
                    IsFound = false,
                    ErrorMessage = "तक्रारीची माहिती मिळवताना त्रुटी झाली. कृपया पुन्हा प्रयत्न करा / Error occurred while tracking application. Please try again."
                };
            }
        }
    }
}
