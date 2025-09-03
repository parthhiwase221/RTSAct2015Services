using RTSAct2015Services.Models.DTOs;
using RTSAct2015Services.Models.Entities;

namespace RTSAct2015Services.Interfaces.IRepository
{
    public interface IDepositRefundRepository
    {
        Task<string> InsertApplicationAsync(DepositRefundCreateDto dto);
        Task<ApplicationEntity?> GetApplicationByIdAsync(string applicationId);
        Task<IEnumerable<ApplicationEntity>> GetApplicationsListAsync(
            int pageNumber = 1,
            int pageSize = 10,
            string? status = null,
            string? priority = null,
            string? searchText = null);
        Task<bool> UpdateApplicationAsync(string applicationId, DepositRefundCreateDto dto);
        Task<bool> DeleteApplicationAsync(string applicationId);
    }
}
