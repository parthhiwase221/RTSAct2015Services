using RTSAct2015.Models.DTOs;
using RTSAct2015.Data.Interfaces;
using System.Data;
using Microsoft.Data.SqlClient;

namespace RTSAct2015.Data.Repositories
{
    public class DeathCertificateRepository : IDeathCertificateRepository
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<DeathCertificateRepository> _logger;

        public DeathCertificateRepository(IConfiguration configuration, ILogger<DeathCertificateRepository> logger)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<(bool Success, string ApplicationId, string Message)> CreateDeathCertificateAsync(
            DeathCertificateCreateDto dto, Dictionary<string, string> filePaths)
        {
            if (dto == null)
            {
                return (false, string.Empty, "Invalid application data");
            }

            filePaths ??= new Dictionary<string, string>();

            string connectionString = _configuration.GetConnectionString("DefaultConnection") ??
                throw new InvalidOperationException("Connection string 'DefaultConnection' not found");

            string applicationId = string.Empty;
            int newId = 0;

            try
            {
                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                using var transaction = connection.BeginTransaction();

                try
                {
                    using var command = new SqlCommand("usp_ManageApplication", connection, transaction)
                    {
                        CommandType = CommandType.StoredProcedure,
                        CommandTimeout = 120
                    };

                    // Basic Application Info
                    command.Parameters.AddWithValue("@Action", "INSERT");
                    command.Parameters.AddWithValue("@ApplicationID", DBNull.Value);
                    command.Parameters.AddWithValue("@ApplicationType", "DEATH_CERTIFICATE");
                    command.Parameters.AddWithValue("@FormName", "Death Certificate Application");
                    command.Parameters.AddWithValue("@Status", "Submitted");
                    command.Parameters.AddWithValue("@Priority", "Medium");
                    command.Parameters.AddWithValue("@CreatedDate", DateTime.Now);
                    command.Parameters.AddWithValue("@IsActive", true);

                    // Applicant Info
                    command.Parameters.AddWithValue("@Title", dto.ApplicantTitle ?? string.Empty);
                    command.Parameters.AddWithValue("@FirstName", dto.ApplicantFirstName ?? string.Empty);
                    command.Parameters.AddWithValue("@MiddleName",
                        string.IsNullOrWhiteSpace(dto.ApplicantMiddleName) ? DBNull.Value : dto.ApplicantMiddleName);
                    command.Parameters.AddWithValue("@LastName", dto.ApplicantLastName ?? string.Empty);
                    command.Parameters.AddWithValue("@Mobile", dto.Mobile ?? string.Empty);
                    command.Parameters.AddWithValue("@Email", DBNull.Value);

                    // Address Info
                    command.Parameters.AddWithValue("@Street", dto.Street ?? string.Empty);
                    command.Parameters.AddWithValue("@Area", dto.Area ?? string.Empty);
                    command.Parameters.AddWithValue("@City", dto.City ?? string.Empty);
                    command.Parameters.AddWithValue("@District", dto.District ?? string.Empty);
                    command.Parameters.AddWithValue("@PinCode", dto.PinCode ?? string.Empty);
                    command.Parameters.AddWithValue("@Landmark", DBNull.Value);
                    command.Parameters.AddWithValue("@Latitude", dto.Latitude);
                    command.Parameters.AddWithValue("@Longitude", dto.Longitude);

                    // Death Certificate Specific Fields
                    command.Parameters.AddWithValue("@DeceasedFullName", dto.DeceasedFullName ?? string.Empty);
                    command.Parameters.AddWithValue("@DateOfDeath", dto.DateOfDeath);
                    command.Parameters.AddWithValue("@TimeOfDeath", dto.TimeOfDeath);
                    command.Parameters.AddWithValue("@Gender", dto.Gender ?? string.Empty);
                    command.Parameters.AddWithValue("@AgeAtDeath", dto.AgeAtDeath);
                    command.Parameters.AddWithValue("@PlaceOfDeath", dto.PlaceOfDeath ?? string.Empty);
                    command.Parameters.AddWithValue("@CauseOfDeath", dto.CauseOfDeath ?? string.Empty);
                    command.Parameters.AddWithValue("@HospitalName",
                        string.IsNullOrWhiteSpace(dto.HospitalName) ? DBNull.Value : dto.HospitalName);
                    command.Parameters.AddWithValue("@RelationshipWithDeceased", dto.RelationshipWithDeceased ?? string.Empty);
                    command.Parameters.AddWithValue("@NumberOfCopies", dto.NumberOfCopies);
                    command.Parameters.AddWithValue("@Purpose", dto.Purpose ?? string.Empty);

                    // Document Files
                    command.Parameters.AddWithValue("@MedicalCertificate",
                        filePaths.TryGetValue("MedicalCertificate", out string? medicalPath) && !string.IsNullOrEmpty(medicalPath)
                            ? medicalPath : DBNull.Value);
                    command.Parameters.AddWithValue("@IdProofDocument",
                        filePaths.TryGetValue("IdProofDocument", out string? idProofPath) && !string.IsNullOrEmpty(idProofPath)
                            ? idProofPath : DBNull.Value);
                    command.Parameters.AddWithValue("@AddressProofDocument",
                        filePaths.TryGetValue("AddressProofDocument", out string? addressProofPath) && !string.IsNullOrEmpty(addressProofPath)
                            ? addressProofPath : DBNull.Value);
                    command.Parameters.AddWithValue("@AdditionalDocument",
                        filePaths.TryGetValue("AdditionalDocument", out string? additionalPath) && !string.IsNullOrEmpty(additionalPath)
                            ? additionalPath : DBNull.Value);

                    // Output parameter
                    var outputParam = new SqlParameter("@NewID", SqlDbType.Int) { Direction = ParameterDirection.Output };
                    command.Parameters.Add(outputParam);

                    await command.ExecuteNonQueryAsync();

                    if (outputParam.Value != null && outputParam.Value != DBNull.Value)
                    {
                        newId = Convert.ToInt32(outputParam.Value);
                    }
                    else
                    {
                        throw new InvalidOperationException("Failed to retrieve new application ID from stored procedure");
                    }

                    // Get generated ApplicationID
                    using var selectCommand = new SqlCommand("SELECT ApplicationID FROM Applications WHERE ID = @ID", connection, transaction);
                    selectCommand.Parameters.AddWithValue("@ID", newId);
                    var result = await selectCommand.ExecuteScalarAsync();
                    applicationId = result?.ToString() ?? string.Empty;

                    if (string.IsNullOrEmpty(applicationId))
                    {
                        throw new InvalidOperationException("Failed to retrieve application ID from database");
                    }

                    await transaction.CommitAsync();

                    _logger.LogInformation("Death certificate application created successfully with ID: {ApplicationId}", applicationId);
                    return (true, applicationId, "Death certificate application created successfully");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Error during death certificate creation transaction");
                    throw;
                }
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "Database error occurred while creating death certificate application");
                return (false, string.Empty, "Database error occurred. Please try again later.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred while creating death certificate application");
                return (false, string.Empty, $"An error occurred: {ex.Message}");
            }
        }
    }
}
