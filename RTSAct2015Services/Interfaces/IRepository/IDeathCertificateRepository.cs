using RTSAct2015.Models.DTOs;

namespace RTSAct2015.Data.Interfaces
{
    public interface IDeathCertificateRepository
    {
        Task<(bool Success, string ApplicationId, string Message)> CreateDeathCertificateAsync(
            DeathCertificateCreateDto dto,
            Dictionary<string, string> filePaths);
    }
}
