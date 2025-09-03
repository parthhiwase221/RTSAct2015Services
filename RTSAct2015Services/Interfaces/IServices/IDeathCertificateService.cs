using RTSAct2015.Models.DTOs;

namespace RTSAct2015.Services
{
    public interface IDeathCertificateService
    {
        Task<(bool Success, string ApplicationId, string Message)> CreateDeathCertificateAsync(DeathCertificateCreateDto dto);
    }
}
