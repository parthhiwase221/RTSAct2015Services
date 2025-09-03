using RTSAct2015Services.Models.DTOs;
using RTSAct2015Services.Models.Entities;

namespace RTSAct2015Services.Interfaces.IServices
{
    public interface ITreeFellingService
    {
        Task<ResponseDto<ApplicationResponseDto>> CreateApplicationAsync(TreeFellingCreateDto createDto);
        Task<ResponseDto<ApplicationEntity>> GetApplicationByIdAsync(string applicationId);
        Task<ResponseDto<IEnumerable<ApplicationEntity>>> GetApplicationsListAsync(
            int pageNumber = 1,
            int pageSize = 10,
            string? status = null,
            string? priority = null,
            string? searchText = null);
        Task<ResponseDto<bool>> UpdateApplicationAsync(string applicationId, TreeFellingCreateDto updateDto);
        Task<ResponseDto<bool>> DeleteApplicationAsync(string applicationId, string deletedBy);
        string GenerateApplicationId();
        Task<string> SaveDocumentAsync(IFormFile file, string subfolder);
    }
}
