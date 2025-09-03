using RTSAct2015.Models.DTOs;
using RTSAct2015.Data.Interfaces;
using System.Data;
using Microsoft.Data.SqlClient;

namespace RTSAct2015.Data.Repositories
{
    public class MarriageCertificateRepository : IMarriageCertificateRepository
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<MarriageCertificateRepository> _logger;

        public MarriageCertificateRepository(IConfiguration configuration, ILogger<MarriageCertificateRepository> logger)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<(bool Success, string ApplicationId, string Message)> CreateMarriageCertificateAsync(
    MarriageCertificateCreateDto dto, Dictionary<string, string> filePaths)
        {
            if (dto == null)
            {
                return (false, string.Empty, "Invalid application data");
            }

            filePaths ??= new Dictionary<string, string>();

            try
            {
                string connectionString = _configuration.GetConnectionString("DefaultConnection") ??
                    throw new InvalidOperationException("Connection string 'DefaultConnection' not found");

                string applicationId = string.Empty;
                int newId = 0;

                using var connection = new SqlConnection(connectionString);
                await connection.OpenAsync();

                // ✅ Log connection success
                _logger.LogInformation("Database connection opened successfully");

                using var transaction = connection.BeginTransaction();

                try
                {
                    using var command = new SqlCommand("usp_ManageApplication", connection, transaction)
                    {
                        CommandType = CommandType.StoredProcedure,
                        CommandTimeout = 120
                    };

                    // ✅ Log stored procedure call
                    _logger.LogInformation("Calling usp_ManageApplication with ApplicationType: MARRIAGE_CERTIFICATE");

                    // Basic Application Info
                    command.Parameters.AddWithValue("@Action", "INSERT");
                    command.Parameters.AddWithValue("@ApplicationID", DBNull.Value);
                    command.Parameters.AddWithValue("@ApplicationType", "MARRIAGE_CERTIFICATE");
                    command.Parameters.AddWithValue("@FormName", "Marriage Certificate Application");
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
                    command.Parameters.AddWithValue("@Email", string.IsNullOrWhiteSpace(dto.Email) ? DBNull.Value : dto.Email);

                    // Marriage Certificate specific fields
                    command.Parameters.AddWithValue("@Zone", dto.Zone ?? string.Empty);
                    command.Parameters.AddWithValue("@MarriageDate", dto.MarriageDate);
                    command.Parameters.AddWithValue("@PersonalLaw", dto.PersonalLaw ?? string.Empty);
                    command.Parameters.AddWithValue("@PlaceOfMarriage", dto.PlaceOfMarriage ?? string.Empty);

                    // Address Info
                    command.Parameters.AddWithValue("@ApplicantAddress", dto.ApplicantAddress ?? string.Empty);
                    command.Parameters.AddWithValue("@Street", DBNull.Value);
                    command.Parameters.AddWithValue("@Area", DBNull.Value);
                    command.Parameters.AddWithValue("@City", DBNull.Value);
                    command.Parameters.AddWithValue("@District", DBNull.Value);
                    command.Parameters.AddWithValue("@PinCode", DBNull.Value);
                    command.Parameters.AddWithValue("@Landmark", DBNull.Value);
                    command.Parameters.AddWithValue("@Latitude", dto.Latitude);
                    command.Parameters.AddWithValue("@Longitude", dto.Longitude);

                    // Groom Details
                    command.Parameters.AddWithValue("@GroomName", dto.GroomName ?? string.Empty);
                    command.Parameters.AddWithValue("@GroomAlternateName",
                        string.IsNullOrWhiteSpace(dto.GroomAlternateName) ? DBNull.Value : dto.GroomAlternateName);
                    command.Parameters.AddWithValue("@GroomReligionBirth", dto.GroomReligionBirth ?? string.Empty);
                    command.Parameters.AddWithValue("@GroomBirthDate", dto.GroomBirthDate);
                    command.Parameters.AddWithValue("@GroomAge", dto.GroomAge);
                    command.Parameters.AddWithValue("@GroomMaritalStatus", dto.GroomMaritalStatus ?? string.Empty);
                    command.Parameters.AddWithValue("@GroomOccupation", dto.GroomOccupation ?? string.Empty);
                    command.Parameters.AddWithValue("@GroomAddress", dto.GroomAddress ?? string.Empty);

                    // Bride Details
                    command.Parameters.AddWithValue("@BrideName", dto.BrideName ?? string.Empty);
                    command.Parameters.AddWithValue("@BrideAlternateName",
                        string.IsNullOrWhiteSpace(dto.BrideAlternateName) ? DBNull.Value : dto.BrideAlternateName);
                    command.Parameters.AddWithValue("@BrideReligionBirth", dto.BrideReligionBirth ?? string.Empty);
                    command.Parameters.AddWithValue("@BrideBirthDate", dto.BrideBirthDate);
                    command.Parameters.AddWithValue("@BrideAge", dto.BrideAge);
                    command.Parameters.AddWithValue("@BrideMaritalStatus", dto.BrideMaritalStatus ?? string.Empty);
                    command.Parameters.AddWithValue("@BrideOccupation", dto.BrideOccupation ?? string.Empty);
                    command.Parameters.AddWithValue("@BrideAddress", dto.BrideAddress ?? string.Empty);

                    // Priest Details
                    command.Parameters.AddWithValue("@PriestName", dto.PriestName ?? string.Empty);
                    command.Parameters.AddWithValue("@PriestReligion", dto.PriestReligion ?? string.Empty);
                    command.Parameters.AddWithValue("@PriestAge", dto.PriestAge);
                    command.Parameters.AddWithValue("@PriestAddress", dto.PriestAddress ?? string.Empty);

                    // ✅ Add NULL values for Birth/Death Certificate fields to avoid missing parameter errors
                    command.Parameters.AddWithValue("@ChildName", DBNull.Value);
                    command.Parameters.AddWithValue("@Gender", DBNull.Value);
                    command.Parameters.AddWithValue("@DateOfBirth", DBNull.Value);
                    command.Parameters.AddWithValue("@PlaceOfBirth", DBNull.Value);
                    command.Parameters.AddWithValue("@HospitalName", DBNull.Value);
                    command.Parameters.AddWithValue("@FatherTitle", DBNull.Value);
                    command.Parameters.AddWithValue("@FatherFirstName", DBNull.Value);
                    command.Parameters.AddWithValue("@FatherLastName", DBNull.Value);
                    command.Parameters.AddWithValue("@MotherTitle", DBNull.Value);
                    command.Parameters.AddWithValue("@MotherFirstName", DBNull.Value);
                    command.Parameters.AddWithValue("@MotherLastName", DBNull.Value);
                    command.Parameters.AddWithValue("@RelationshipWithChild", DBNull.Value);
                    command.Parameters.AddWithValue("@NumberOfCopies", DBNull.Value);
                    command.Parameters.AddWithValue("@Purpose", DBNull.Value);
                    command.Parameters.AddWithValue("@DeceasedFullName", DBNull.Value);
                    command.Parameters.AddWithValue("@DateOfDeath", DBNull.Value);
                    command.Parameters.AddWithValue("@TimeOfDeath", DBNull.Value);
                    command.Parameters.AddWithValue("@AgeAtDeath", DBNull.Value);
                    command.Parameters.AddWithValue("@PlaceOfDeath", DBNull.Value);
                    command.Parameters.AddWithValue("@CauseOfDeath", DBNull.Value);
                    command.Parameters.AddWithValue("@RelationshipWithDeceased", DBNull.Value);

                    // Document Files
                    command.Parameters.AddWithValue("@DischargeDocument", DBNull.Value);
                    command.Parameters.AddWithValue("@IdProofDocument", DBNull.Value);
                    command.Parameters.AddWithValue("@AddressProofDocument", DBNull.Value);
                    command.Parameters.AddWithValue("@AdditionalDocument", DBNull.Value);
                    command.Parameters.AddWithValue("@MedicalCertificate", DBNull.Value);

                    command.Parameters.AddWithValue("@GroomPhoto",
                        filePaths.TryGetValue("GroomPhoto", out string? groomPhotoPath) && !string.IsNullOrEmpty(groomPhotoPath)
                            ? groomPhotoPath : DBNull.Value);
                    command.Parameters.AddWithValue("@BridePhoto",
                        filePaths.TryGetValue("BridePhoto", out string? bridePhotoPath) && !string.IsNullOrEmpty(bridePhotoPath)
                            ? bridePhotoPath : DBNull.Value);
                    command.Parameters.AddWithValue("@PriestPhoto",
                        filePaths.TryGetValue("PriestPhoto", out string? priestPhotoPath) && !string.IsNullOrEmpty(priestPhotoPath)
                            ? priestPhotoPath : DBNull.Value);
                    command.Parameters.AddWithValue("@MarriageCard",
                        filePaths.TryGetValue("MarriageCard", out string? marriageCardPath) && !string.IsNullOrEmpty(marriageCardPath)
                            ? marriageCardPath : DBNull.Value);
                    command.Parameters.AddWithValue("@GroupPhoto",
                        filePaths.TryGetValue("GroupPhoto", out string? groupPhotoPath) && !string.IsNullOrEmpty(groupPhotoPath)
                            ? groupPhotoPath : DBNull.Value);

                    // Output parameter
                    var outputParam = new SqlParameter("@NewID", SqlDbType.Int) { Direction = ParameterDirection.Output };
                    command.Parameters.Add(outputParam);

                    // ✅ Log before executing
                    _logger.LogInformation("About to execute stored procedure with {ParameterCount} parameters", command.Parameters.Count);

                    await command.ExecuteNonQueryAsync();

                    // ✅ Log after execution
                    _logger.LogInformation("Stored procedure executed successfully");

                    if (outputParam.Value != null && outputParam.Value != DBNull.Value)
                    {
                        newId = Convert.ToInt32(outputParam.Value);
                        _logger.LogInformation("Generated new ID: {NewId}", newId);
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

                    _logger.LogInformation("Marriage certificate application created successfully with ID: {ApplicationId}", applicationId);
                    return (true, applicationId, "Marriage certificate application created successfully");
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Error during marriage certificate creation transaction: {ErrorMessage}", ex.Message);
                    throw; // Re-throw to get caught by outer catch
                }
            }
            catch (SqlException ex)
            {
                _logger.LogError(ex, "SQL Database error occurred: {ErrorMessage} | ErrorNumber: {ErrorNumber}", ex.Message, ex.Number);
                return (false, string.Empty, $"SQL Error {ex.Number}: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error occurred: {ErrorMessage}", ex.Message);
                return (false, string.Empty, $"Error: {ex.Message}");
            }
        }

    }
}
