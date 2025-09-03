using Dapper;
using Microsoft.Data.SqlClient;
using RTSAct2015Services.Interfaces.IRepository;
using RTSAct2015Services.Models.DTOs;
using RTSAct2015Services.Models.Entities;
using System.Data;

namespace RTSAct2015Services.Data.Repositories
{
    public class TreeTrimmingRepository : ITreeTrimmingRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public TreeTrimmingRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentException("Connection string not found");
        }

        private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

        public async Task<string> InsertApplicationAsync(TreeTrimmingCreateDto dto)
        {
            using var connection = CreateConnection();

            var applicationId = GenerateApplicationId(); // This will be ignored by stored procedure
            var parameters = new DynamicParameters();

            // Action and Application Info
            parameters.Add("@Action", "INSERT", DbType.String);
            parameters.Add("@ApplicationID", applicationId, DbType.String);
            parameters.Add("@ApplicationType", "TREE_TRIMMING", DbType.String);
            parameters.Add("@FormName", "Tree Trimming Application", DbType.String);
            parameters.Add("@Status", "Submitted", DbType.String);
            parameters.Add("@Priority", "Medium", DbType.String);
            parameters.Add("@IsActive", true, DbType.Boolean);

            // Personal Details
            parameters.Add("@Title", dto.Title, DbType.String);
            parameters.Add("@FirstName", dto.FirstName, DbType.String);
            parameters.Add("@MiddleName", dto.MiddleName, DbType.String);
            parameters.Add("@LastName", dto.LastName, DbType.String);
            parameters.Add("@Mobile", dto.Mobile, DbType.String);
            parameters.Add("@Email", dto.Email, DbType.String);
            parameters.Add("@Street", dto.Street, DbType.String);
            parameters.Add("@Area", dto.Area, DbType.String);
            parameters.Add("@City", dto.City, DbType.String);
            parameters.Add("@PinCode", dto.PinCode, DbType.String);
            parameters.Add("@Landmark", dto.Landmark, DbType.String);
            parameters.Add("@Latitude", dto.Latitude, DbType.Decimal);
            parameters.Add("@Longitude", dto.Longitude, DbType.Decimal);

            // Tree Trimming Specific Fields
            parameters.Add("@ReasonForTrimming", dto.ReasonForTrimming, DbType.String);
            parameters.Add("@TreeType", dto.TreeType, DbType.String);
            parameters.Add("@OwnerType", dto.OwnerType, DbType.String);
            parameters.Add("@TypeOfApplicant", dto.TypeOfApplicant, DbType.String);
            parameters.Add("@TreeCount", dto.TreeCount, DbType.Int32);
            parameters.Add("@Species", dto.TreeSpecies, DbType.String);
            parameters.Add("@OtherReason", dto.OtherReason, DbType.String);

            // Document Paths
            parameters.Add("@DocumentPath", dto.DocumentPath, DbType.String);
            parameters.Add("@DocumentName", dto.DocumentName, DbType.String);
            parameters.Add("@DocumentType", dto.DocumentType, DbType.String);
            parameters.Add("@DocumentSize", dto.DocumentSize, DbType.Int64);

            // File Paths
            parameters.Add("@PropertyTaxReceiptFile", dto.PropertyTaxReceiptFilePath, DbType.String);
            parameters.Add("@TreePhotoFile", dto.TreePhotographFilePath, DbType.String);
            parameters.Add("@AadhaarFile", dto.AadhaarCardFilePath, DbType.String);
            parameters.Add("@BuildingPermissionFile", dto.BuildingPermissionFilePath, DbType.String);
            parameters.Add("@SanctionedPlanFile", dto.SanctionedPlanFilePath, DbType.String);
            parameters.Add("@NOCFile", dto.NOCLetterFilePath, DbType.String);

            // Terms
            parameters.Add("@TermsAccepted", $"Term1:{dto.TermsCondition1},Term2:{dto.TermsCondition2},Term3:{dto.TermsCondition3},Term4:{dto.TermsCondition4},Term5:{dto.TermsCondition5},Term6:{dto.TermsCondition6}", DbType.String);
            parameters.Add("@SelectAllTerms", dto.SelectAllTerms, DbType.Boolean);

            parameters.Add("@NewID", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await connection.ExecuteAsync("usp_ManageApplication", parameters, commandType: CommandType.StoredProcedure);

            // ✅ FIXED: Get the complaint number (which is now stored as ApplicationID)
            var complaintNumber = await connection.QuerySingleOrDefaultAsync<string>(
                "SELECT TOP 1 ApplicationID FROM Applications WHERE ApplicationType = 'TREE_TRIMMING' ORDER BY ID DESC"
            );

            return complaintNumber ?? "TTR00000";
        }

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
                WHERE ApplicationType = 'TREE_TRIMMING' AND IsActive = 1
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

        public async Task<bool> UpdateApplicationAsync(string applicationId, TreeTrimmingCreateDto dto)
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
            string? priority = null,
            string? searchText = null)
        {
            using var connection = CreateConnection();

            var sql = "SELECT COUNT(1) FROM Applications WHERE ApplicationType = 'TREE_TRIMMING' AND IsActive = 1";

            var count = await connection.ExecuteScalarAsync<int>(sql);
            return count;
        }

        private string GenerateApplicationId()
        {
            var now = DateTime.Now;
            return $"TTR{now:yyyyMMdd}-{now:HHmmss}-{Random.Shared.Next(100, 999)}";
        }
    }
}
