using RTSAct2015.Models.DTOs;

namespace RTSAct2015.Services
{
    public interface IBirthCertificateService
    {
        Task<(bool Success, string ApplicationId, string Message)> CreateBirthCertificateAsync(BirthCertificateCreateDto dto);
    }
}
