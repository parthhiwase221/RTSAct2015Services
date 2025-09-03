using Dapper;
using Microsoft.Data.SqlClient;
using RTSAct2015Services.Interfaces.IRepository;
using RTSAct2015Services.Models.DTOs;
using RTSAct2015Services.Models.Entities;
using System.Data;

namespace RTSAct2015Services.Data.Repositories
{
    public class OFCPermissionRepository : IOFCPermissionRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;
        private readonly ILogger<OFCPermissionRepository> _logger;

        public OFCPermissionRepository(IConfiguration configuration, ILogger<OFCPermissionRepository> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _connectionString = _configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentException("Connection string not found");
        }

        private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

        public async Task<string> InsertApplicationAsync(OFCPermissionCreateDto dto)
        {
            using var connection = CreateConnection();

            try
            {
                var applicationId = GenerateApplicationId();
                var parameters = new DynamicParameters();

                _logger.LogInformation("Starting OFC repository insert with ApplicationID: {ApplicationId}", applicationId);

                parameters.Add("@Action", "INSERT", DbType.String);
                parameters.Add("@ApplicationID", applicationId, DbType.String);
                parameters.Add("@ApplicationType", "OFC_PERMISSION", DbType.String);
                parameters.Add("@FormName", "OFC Installation Permission", DbType.String);
                parameters.Add("@Status", "Submitted", DbType.String);
                parameters.Add("@Priority", "Medium", DbType.String);
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

                // OFC Specific Fields
                parameters.Add("@InstallationType", dto.InstallationType ?? "", DbType.String);
                parameters.Add("@CableType", dto.CableType ?? "", DbType.String);
                parameters.Add("@CompanyName", dto.CompanyName ?? "", DbType.String);
                parameters.Add("@CompanyRegNo", dto.CompanyRegNo, DbType.String);
                parameters.Add("@AuthorizedRep", dto.AuthorizedRep, DbType.String);
                parameters.Add("@TotalLength", dto.TotalLength, DbType.Decimal);
                parameters.Add("@TrenchWidth", dto.TrenchWidth, DbType.Decimal);
                parameters.Add("@PlaceName", dto.PlaceName, DbType.String);
                parameters.Add("@WorkType", dto.WorkType, DbType.String);

                // Document Paths
                parameters.Add("@DocumentPath", dto.DocumentPath, DbType.String);
                parameters.Add("@DocumentName", dto.DocumentName, DbType.String);
                parameters.Add("@DocumentType", dto.DocumentType, DbType.String);
                parameters.Add("@DocumentSize", dto.DocumentSize, DbType.Int64);

                // Terms
                parameters.Add("@TermsAccepted", "Term1:True,Term2:True,Term3:True,Term4:True,Term5:True", DbType.String);
                parameters.Add("@SelectAllTerms", true, DbType.Boolean);

                parameters.Add("@NewID", dbType: DbType.Int32, direction: ParameterDirection.Output);

                await connection.ExecuteAsync("usp_ManageApplication", parameters, commandType: CommandType.StoredProcedure);

                // ✅ NEW: Get the complaint number (stored as ApplicationID)
                var complaintNumber = await connection.QuerySingleOrDefaultAsync<string>(
                    "SELECT TOP 1 ApplicationID FROM Applications WHERE ApplicationType = 'OFC_PERMISSION' ORDER BY ID DESC"
                );

                return complaintNumber ?? "OFC00000";
            }
            catch (SqlException sqlEx)
            {
                _logger.LogError(sqlEx, "SQL Exception in OFC Repository - Number: {Number}, Severity: {Severity}, State: {State}, Procedure: {Procedure}, Message: {Message}",
                    sqlEx.Number, sqlEx.Class, sqlEx.State, sqlEx.Procedure, sqlEx.Message);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "General exception in OFC Repository: {Message}", ex.Message);
                throw;
            }
        }

        // All other methods remain unchanged
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
            string? priority = null,
            string? searchText = null)
        {
            using var connection = CreateConnection();

            var sql = @"
                SELECT * FROM Applications 
                WHERE ApplicationType = 'OFC_PERMISSION' AND IsActive = 1
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

        public async Task<bool> UpdateApplicationAsync(string applicationId, OFCPermissionCreateDto dto)
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

        private string GenerateApplicationId()
        {
            var now = DateTime.Now;
            return $"OFC{now:yyyyMMdd}-{now:HHmmss}-{Random.Shared.Next(100, 999)}";
        }
    }
}
