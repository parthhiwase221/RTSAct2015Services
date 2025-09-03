using RTSAct2015.Models.DTOs;

namespace RTSAct2015.Data.Interfaces
{
    public interface IMarriageCertificateRepository
    {
        Task<(bool Success, string ApplicationId, string Message)> CreateMarriageCertificateAsync(
            MarriageCertificateCreateDto dto,
            Dictionary<string, string> filePaths);
    }
}
