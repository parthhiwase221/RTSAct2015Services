using Dapper;
using Microsoft.Data.SqlClient;
using RTSAct2015Services.Interfaces.IRepository;
using RTSAct2015Services.Models.Entities;
using System.Data;

namespace RTSAct2015Services.Data.Repositories
{
    public class TrackApplicationRepository : ITrackApplicationRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        private readonly ILogger<TrackApplicationRepository> _logger;

        public TrackApplicationRepository(IConfiguration configuration, ILogger<TrackApplicationRepository> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _connectionString = _configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentException("Connection string not found");
        }

        private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

        public async Task<ApplicationTrackingEntity?> GetApplicationByComplaintNumberAsync(string complaintNumber)
        {
            using var connection = CreateConnection();

            try
            {
                var sql = @"
                    SELECT TOP 1 * FROM Applications 
                    WHERE ApplicationID = @ComplaintNumber AND IsActive = 1
                    ORDER BY CreatedDate DESC";

                var result = await connection.QueryFirstOrDefaultAsync<ApplicationTrackingEntity>(sql, new { ComplaintNumber = complaintNumber });

                _logger.LogInformation("Track query executed for complaint number: {ComplaintNumber}, Found: {Found}", complaintNumber, result != null);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error tracking application by complaint number: {ComplaintNumber}", complaintNumber);
                throw;
            }
        }

        public async Task<ApplicationTrackingEntity?> GetApplicationByComplaintNumberAndMobileAsync(string complaintNumber, string mobileNumber)
        {
            using var connection = CreateConnection();

            try
            {
                var sql = @"
                    SELECT TOP 1 * FROM Applications 
                    WHERE ApplicationID = @ComplaintNumber 
                    AND Mobile = @MobileNumber 
                    AND IsActive = 1
                    ORDER BY CreatedDate DESC";

                var result = await connection.QueryFirstOrDefaultAsync<ApplicationTrackingEntity>(sql,
                    new { ComplaintNumber = complaintNumber, MobileNumber = mobileNumber });

                _logger.LogInformation("Track query executed for complaint number: {ComplaintNumber} with mobile: {Mobile}, Found: {Found}",
                    complaintNumber, mobileNumber, result != null);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error tracking application by complaint number and mobile: {ComplaintNumber}", complaintNumber);
                throw;
            }
        }

        public async Task<IEnumerable<ApplicationTrackingEntity>> GetApplicationsByMobileNumberAsync(string mobileNumber)
        {
            using var connection = CreateConnection();

            try
            {
                var sql = @"
                    SELECT * FROM Applications 
                    WHERE Mobile = @MobileNumber AND IsActive = 1
                    ORDER BY CreatedDate DESC";

                var results = await connection.QueryAsync<ApplicationTrackingEntity>(sql, new { MobileNumber = mobileNumber });

                _logger.LogInformation("Track query executed for mobile: {Mobile}, Found: {Count} applications", mobileNumber, results.Count());

                return results;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting applications by mobile number: {Mobile}", mobileNumber);
                throw;
            }
        }
    }
}
