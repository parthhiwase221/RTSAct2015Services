using RTSAct2015.Models.DTOs;

namespace RTSAct2015.Data.Interfaces
{
    public interface IBirthCertificateRepository
    {
        Task<(bool Success, string ApplicationId, string Message)> CreateBirthCertificateAsync(BirthCertificateCreateDto dto, Dictionary<string, string> filePaths);
    }
}
