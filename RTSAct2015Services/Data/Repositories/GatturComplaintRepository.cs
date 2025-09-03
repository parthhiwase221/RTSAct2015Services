using Dapper;
using Microsoft.Data.SqlClient;
using RTSAct2015Services.Interfaces.IRepository;
using RTSAct2015Services.Models.DTOs;
using RTSAct2015Services.Models.Entities;
using System.Data;

namespace RTSAct2015Services.Data.Repositories
{
    public class GatturComplaintRepository : IGatturComplaintRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        private readonly ILogger<GatturComplaintRepository> _logger;

        public GatturComplaintRepository(IConfiguration configuration, ILogger<GatturComplaintRepository> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _connectionString = _configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentException("Connection string not found");
        }

        private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

        public async Task<string> InsertApplicationAsync(GatturComplaintCreateDto dto)
        {
            using var connection = CreateConnection();

            try
            {
                var applicationId = GenerateApplicationId(); // This will be ignored by stored procedure
                var parameters = new DynamicParameters();

                _logger.LogInformation("Starting Gattur complaint repository insert with ApplicationID: {ApplicationId}", applicationId);

                // Action and Application Info
                parameters.Add("@Action", "INSERT", DbType.String);
                parameters.Add("@ApplicationID", applicationId, DbType.String);
                parameters.Add("@ApplicationType", "GUT", DbType.String);
                parameters.Add("@FormName", "Gutter Repair Complaint", DbType.String);
                parameters.Add("@Status", "Submitted", DbType.String);
                parameters.Add("@Priority", "High", DbType.String);
                parameters.Add("@IsActive", true, DbType.Boolean);

                // Personal Details
                parameters.Add("@Title", dto.Title ?? "", DbType.String);
                parameters.Add("@FirstName", dto.FirstName ?? "", DbType.String);
                parameters.Add("@MiddleName", dto.MiddleName, DbType.String);
                parameters.Add("@LastName", dto.LastName ?? "", DbType.String);
                parameters.Add("@Mobile", dto.Mobile ?? "", DbType.String);
                parameters.Add("@Email", dto.Email, DbType.String);
                parameters.Add("@Street", dto.Street ?? "", DbType.String);
                parameters.Add("@Area", dto.Area ?? "", DbType.String);
                parameters.Add("@City", dto.City ?? "", DbType.String);
                parameters.Add("@PinCode", dto.PinCode ?? "", DbType.String);
                parameters.Add("@Landmark", dto.Landmark ?? "", DbType.String);
                parameters.Add("@Latitude", dto.Latitude, DbType.Decimal);
                parameters.Add("@Longitude", dto.Longitude, DbType.Decimal);
                parameters.Add("@ZoneType", dto.ZoneType ?? "", DbType.String);

                // Document Paths
                parameters.Add("@DocumentPath", dto.DocumentPath, DbType.String);
                parameters.Add("@DocumentName", dto.DocumentName, DbType.String);
                parameters.Add("@DocumentType", dto.DocumentType, DbType.String);
                parameters.Add("@DocumentSize", dto.DocumentSize, DbType.Int64);

                parameters.Add("@NewID", dbType: DbType.Int32, direction: ParameterDirection.Output);

                _logger.LogInformation("Calling stored procedure usp_ManageApplication with {ParamCount} parameters", parameters.ParameterNames.Count());

                await connection.ExecuteAsync("usp_ManageApplication", parameters, commandType: CommandType.StoredProcedure);

                var newId = parameters.Get<int>("@NewID");
                _logger.LogInformation("Stored procedure completed successfully. NewID: {NewId}", newId);

                // ✅ NEW: Get the complaint number (which is now stored as ApplicationID)
                var complaintNumber = await connection.QuerySingleOrDefaultAsync<string>(
                    "SELECT TOP 1 ApplicationID FROM Applications WHERE ApplicationType = 'GUT' ORDER BY ID DESC"
                );

                _logger.LogInformation("Generated ComplaintNumber: {ComplaintNumber}", complaintNumber);

                return complaintNumber ?? "GUT00000";
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "SQL Exception in Gattur Repository - Number: {Number}, Message: {Message}",
                    sqlEx.Number, sqlEx.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "General exception in Gattur Repository: {Message}", ex.Message);
                throw;
            }
        }

        // All other methods remain the same
        public async Task<ApplicationEntity?> GetApplicationByIdAsync(string applicationId)
        {
            using var connection = CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@Action", "GETBYID", DbType.String);
            parameters.Add("@ApplicationID", applicationId, DbType.String);
            parameters.Add("@NewID", dbType: DbType.Int32, direction: ParameterDirection.Output);

            var result = await connection.QueryFirstOrDefaultAsync<ApplicationEntity>(
                "usp_ManageApplication",
                parameters,
                commandType: CommandType.StoredProcedure);

            return result;
        }

        public async Task<IEnumerable<ApplicationEntity>> GetApplicationsListAsync(
            int pageNumber = 1,
            int pageSize = 10,
            string? status = null,
            string? zoneType = null,
            string? priority = null,
            string? searchText = null)
        {
            using var connection = CreateConnection();

            var sql = @"
                SELECT * FROM Applications 
                WHERE ApplicationType = 'GUT' AND IsActive = 1
                ORDER BY CreatedDate DESC
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            var parameters = new
            {
                Offset = (pageNumber - 1) * pageSize,
                PageSize = pageSize
            };

            var results = await connection.QueryAsync<ApplicationEntity>(sql, parameters);
            return results;
        }

        public async Task<bool> UpdateApplicationAsync(string applicationId, GatturComplaintCreateDto dto)
        {
            using var connection = CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@Action", "UPDATEFLAG", DbType.String);
            parameters.Add("@ApplicationID", applicationId, DbType.String);
            parameters.Add("@Status", "Updated", DbType.String);
            parameters.Add("@NewID", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await connection.ExecuteAsync("usp_ManageApplication", parameters, commandType: CommandType.StoredProcedure);
            return true;
        }

        public async Task<bool> DeleteApplicationAsync(string applicationId, string deletedBy)
        {
            using var connection = CreateConnection();

            var parameters = new DynamicParameters();
            parameters.Add("@Action", "DELETE", DbType.String);
            parameters.Add("@ApplicationID", applicationId, DbType.String);
            parameters.Add("@NewID", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await connection.ExecuteAsync("usp_ManageApplication", parameters, commandType: CommandType.StoredProcedure);
            return true;
        }

        public async Task<int> GetTotalApplicationsCountAsync(
            string? status = null,
            string? zoneType = null,
            string? priority = null,
            string? searchText = null)
        {
            using var connection = CreateConnection();

            var sql = "SELECT COUNT(1) FROM Applications WHERE ApplicationType = 'GUT' AND IsActive = 1";

            var count = await connection.ExecuteScalarAsync<int>(sql);
            return count;
        }

        private string GenerateApplicationId()
        {
            var now = DateTime.Now;
            return $"GUT{now:yyyyMMdd}-{now:HHmmss}-{Random.Shared.Next(100, 999)}";
        }
    }
}
