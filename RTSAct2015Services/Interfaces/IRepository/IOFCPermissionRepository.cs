using RTSAct2015Services.Models.DTOs;
using RTSAct2015Services.Models.Entities;

namespace RTSAct2015Services.Interfaces.IRepository
{
    public interface IOFCPermissionRepository
    {
        Task<string> InsertApplicationAsync(OFCPermissionCreateDto dto);
        Task<ApplicationEntity?> GetApplicationByIdAsync(string applicationId);
        Task<IEnumerable<ApplicationEntity>> GetApplicationsListAsync(
            int pageNumber = 1,
            int pageSize = 10,
            string? status = null,
            string? priority = null,
            string? searchText = null);
        Task<bool> UpdateApplicationAsync(string applicationId, OFCPermissionCreateDto dto);
        Task<bool> DeleteApplicationAsync(string applicationId, string deletedBy);
    }
}
