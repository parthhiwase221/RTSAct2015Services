using RTSAct2015Services.Models.DTOs;
using RTSAct2015Services.Models.Entities;

namespace RTSAct2015Services.Interfaces.IServices
{
    public interface IDepositRefundService
    {
        Task<ResponseDto<ApplicationResponseDto>> CreateApplicationAsync(DepositRefundCreateDto createDto);
        Task<ResponseDto<ApplicationEntity>> GetApplicationByIdAsync(string applicationId);
        Task<ResponseDto<IEnumerable<ApplicationEntity>>> GetApplicationsListAsync(
            int pageNumber = 1,
            int pageSize = 10,
            string? status = null,
            string? priority = null,
            string? searchText = null);
        Task<ResponseDto<bool>> UpdateApplicationAsync(string applicationId, DepositRefundCreateDto updateDto);
        Task<ResponseDto<bool>> DeleteApplicationAsync(string applicationId);
        Task<string> SaveDocumentAsync(IFormFile file, string subfolder);
    }
}
