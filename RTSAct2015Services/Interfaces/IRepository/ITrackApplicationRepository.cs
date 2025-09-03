using RTSAct2015Services.Models.Entities;

namespace RTSAct2015Services.Interfaces.IRepository
{
    public interface ITrackApplicationRepository
    {
        Task<ApplicationTrackingEntity?> GetApplicationByComplaintNumberAsync(string complaintNumber);
        Task<ApplicationTrackingEntity?> GetApplicationByComplaintNumberAndMobileAsync(string complaintNumber, string mobileNumber);
        Task<IEnumerable<ApplicationTrackingEntity>> GetApplicationsByMobileNumberAsync(string mobileNumber);
    }
}
