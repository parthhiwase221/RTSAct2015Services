using RTSAct2015.Models.DTOs;

namespace RTSAct2015.Services
{
    public interface IMarriageCertificateService
    {
        Task<(bool Success, string ApplicationId, string Message)> CreateMarriageCertificateAsync(MarriageCertificateCreateDto dto);
    }
}
