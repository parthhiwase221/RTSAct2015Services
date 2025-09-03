using Dapper;
using Microsoft.Data.SqlClient;
using RTSAct2015Services.Interfaces.IRepository;
using RTSAct2015Services.Models.DTOs;
using RTSAct2015Services.Models.Entities;
using System.Data;

namespace RTSAct2015Services.Data.Repositories
{
    public class DepositRefundRepository : IDepositRefundRepository
    {
        private readonly IConfiguration _configuration;
        private readonly string _connectionString;

        public DepositRefundRepository(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("DefaultConnection")
                ?? throw new ArgumentException("Connection string not found");
        }

        private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

        public async Task<string> InsertApplicationAsync(DepositRefundCreateDto dto)
        {
            using var connection = CreateConnection();

            var applicationId = GenerateApplicationId(); 
            var parameters = new DynamicParameters();

            // Action and Application Info
            parameters.Add("@Action", "INSERT", DbType.String);
            parameters.Add("@ApplicationID", applicationId, DbType.String);
            parameters.Add("@ApplicationType", "DEPOSIT_REFUND", DbType.String);
            parameters.Add("@FormName", "Security Deposit Refund", DbType.String);
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
            parameters.Add("@ApplicantAddress", dto.ApplicantAddress, DbType.String);
            parameters.Add("@Street", dto.Street, DbType.String);
            parameters.Add("@Area", dto.Area, DbType.String);
            parameters.Add("@City", dto.City, DbType.String);
            parameters.Add("@PinCode", dto.PinCode, DbType.String);
            parameters.Add("@Landmark", dto.Landmark, DbType.String);
            parameters.Add("@Latitude", dto.Latitude, DbType.Decimal);
            parameters.Add("@Longitude", dto.Longitude, DbType.Decimal);

            // Deposit Refund Specific Fields
            parameters.Add("@ApplicantType", dto.ApplicantType, DbType.String);
            parameters.Add("@Zone", dto.ZoneName, DbType.String);
            parameters.Add("@GutNo", dto.GutNo, DbType.String);
            parameters.Add("@SurveyNo", dto.SurveyNo, DbType.String);
            parameters.Add("@CtsNo", dto.CtsNo, DbType.String);
            parameters.Add("@OccupancyCertificateNo", dto.OccupancyCertificateNo, DbType.String);
            parameters.Add("@PropertyAddress", dto.PropertyAddress, DbType.String);
            parameters.Add("@DepositReason", dto.ReasonForDeposit, DbType.String);
            parameters.Add("@PermitFileNo", dto.BuildingPermitFileNo, DbType.String);
            parameters.Add("@BuildingDepositAmount", dto.BuildingDepositAmount, DbType.Decimal);
            parameters.Add("@TreeDepositAmount", dto.TreeDepositAmount, DbType.Decimal);
            parameters.Add("@BankAccountNumber", dto.BankAccountNumber, DbType.String);
            parameters.Add("@BankName", dto.BankName, DbType.String);
            parameters.Add("@IFSCCode", dto.IFSCCode, DbType.String);
            parameters.Add("@ChallanNo", dto.ChallanNo, DbType.String);
            parameters.Add("@ChallanAmount", dto.ChallanAmount, DbType.Decimal);
            parameters.Add("@ChallanPaidDate", dto.ChallanPaidDate, DbType.Date);
            parameters.Add("@HasGardenNOC", dto.HasGardenNOC, DbType.String);
            parameters.Add("@OccupancyCertificateStatus", dto.OccupancyCertificateStatus, DbType.String);
            parameters.Add("@RefundBuildingDeposit", dto.RefundBuildingDeposit, DbType.Boolean);
            parameters.Add("@RefundTreeDeposit", dto.RefundTreeDeposit, DbType.Boolean);
            parameters.Add("@Declaration", dto.Declaration, DbType.Boolean);

            // Document Paths Storage
            parameters.Add("@DocumentPath", dto.DocumentPath, DbType.String);
            parameters.Add("@DocumentName", dto.DocumentName, DbType.String);
            parameters.Add("@DocumentType", dto.DocumentType, DbType.String);
            parameters.Add("@DocumentSize", dto.DocumentSize, DbType.Int64);

            // File Paths for each document type
            parameters.Add("@OccupancyCertFile", dto.OccupancyCertFilePath, DbType.String);
            parameters.Add("@BuildingDepReceiptFile", dto.BuildingDepReceiptFilePath, DbType.String);
            parameters.Add("@TreeDepReceiptFile", dto.TreeDepReceiptFilePath, DbType.String);
            parameters.Add("@PropertyTaxReceiptFile", dto.PropertyTaxReceiptFilePath, DbType.String);
            parameters.Add("@BuildingPermissionCertFile", dto.BuildingPermissionCertFilePath, DbType.String);
            parameters.Add("@ChallanFile", dto.ChallanFilePath, DbType.String);
            parameters.Add("@NOCFile", dto.NOCFilePath, DbType.String);
            parameters.Add("@Notes", dto.Notes, DbType.String);
            parameters.Add("@SelectAll", dto.SelectAll, DbType.Boolean);

            parameters.Add("@NewID", dbType: DbType.Int32, direction: ParameterDirection.Output);

            await connection.ExecuteAsync("usp_ManageApplication", parameters, commandType: CommandType.StoredProcedure);

            // ✅ FIXED: Get the complaint number (which is now stored as ApplicationID)
            var complaintNumber = await connection.QuerySingleOrDefaultAsync<string>(
                "SELECT TOP 1 ApplicationID FROM Applications WHERE ApplicationType = 'DEPOSIT_REFUND' ORDER BY ID DESC"
            );

            return complaintNumber ?? "DEP00000";
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
                WHERE ApplicationType = 'DEPOSIT_REFUND' AND IsActive = 1
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

        public async Task<bool> UpdateApplicationAsync(string applicationId, DepositRefundCreateDto dto)
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

        public async Task<bool> DeleteApplicationAsync(string applicationId)
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

            var sql = "SELECT COUNT(1) FROM Applications WHERE ApplicationType = 'DEPOSIT_REFUND' AND IsActive = 1";

            var count = await connection.ExecuteScalarAsync<int>(sql);
            return count;
        }

        private string GenerateApplicationId()
        {
            var now = DateTime.Now;
            return $"DEP{now:yyyyMMdd}-{now:HHmmss}-{Random.Shared.Next(100, 999)}";
        }
    }
}
