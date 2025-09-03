using RTSAct2015.Models.DTOs;
using RTSAct2015.Data.Interfaces;
using System.Data;
using Microsoft.Data.SqlClient;

namespace RTSAct2015.Data.Repositories
{
    public class BirthCertificateRepository : IBirthCertificateRepository
    {
        private readonly IConfiguration _configuration;

        public BirthCertificateRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<(bool Success, string ApplicationId, string Message)> CreateBirthCertificateAsync(BirthCertificateCreateDto dto, Dictionary<string, string> filePaths)
        {
            try
            {
                string connectionString = _configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string not found");
                string applicationId = string.Empty;
                int newId = 0;

                using (var connection = new SqlConnection(connectionString))
                {
                    await connection.OpenAsync();
                    using (var command = new SqlCommand("usp_ManageApplication", connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        // Basic Application Info
                        command.Parameters.AddWithValue("@Action", "INSERT");
                        command.Parameters.AddWithValue("@ApplicationID", DBNull.Value);
                        command.Parameters.AddWithValue("@ApplicationType", "BIRTH_CERTIFICATE");
                        command.Parameters.AddWithValue("@FormName", "Birth Certificate Application");
                        command.Parameters.AddWithValue("@Status", "Submitted");
                        command.Parameters.AddWithValue("@Priority", "Medium");
                        command.Parameters.AddWithValue("@CreatedDate", DateTime.Now);
                        command.Parameters.AddWithValue("@IsActive", true);

                        // Applicant Basic Info
                        command.Parameters.AddWithValue("@Title", dto.Title);
                        command.Parameters.AddWithValue("@FirstName", dto.FirstName);
                        command.Parameters.AddWithValue("@MiddleName", string.IsNullOrEmpty(dto.MiddleName) ? DBNull.Value : dto.MiddleName);
                        command.Parameters.AddWithValue("@LastName", dto.LastName);
                        command.Parameters.AddWithValue("@Mobile", dto.Mobile);
                        command.Parameters.AddWithValue("@Email", DBNull.Value);

                        // Address Info
                        command.Parameters.AddWithValue("@Street", dto.Street);
                        command.Parameters.AddWithValue("@Area", dto.Area);
                        command.Parameters.AddWithValue("@City", dto.City);
                        command.Parameters.AddWithValue("@District", dto.District);
                        command.Parameters.AddWithValue("@PinCode", dto.PinCode);
                        // Instead of DBNull.Value, use:
                        command.Parameters.AddWithValue("@Landmark", string.IsNullOrEmpty(dto.Landmark) ? DBNull.Value : dto.Landmark);
                        command.Parameters.AddWithValue("@Latitude", dto.Latitude);
                        command.Parameters.AddWithValue("@Longitude", dto.Longitude);

                        // Birth Certificate Specific Fields
                        command.Parameters.AddWithValue("@ChildName", dto.ChildName);
                        command.Parameters.AddWithValue("@Gender", dto.Gender);
                        command.Parameters.AddWithValue("@DateOfBirth", dto.DateOfBirth);
                        command.Parameters.AddWithValue("@PlaceOfBirth", dto.PlaceOfBirth);
                        command.Parameters.AddWithValue("@HospitalName", dto.HospitalName);
                        command.Parameters.AddWithValue("@FatherTitle", dto.FatherTitle);
                        command.Parameters.AddWithValue("@FatherFirstName", dto.FatherFirstName);
                        command.Parameters.AddWithValue("@FatherLastName", dto.FatherLastName);
                        command.Parameters.AddWithValue("@MotherTitle", dto.MotherTitle);
                        command.Parameters.AddWithValue("@MotherFirstName", dto.MotherFirstName);
                        command.Parameters.AddWithValue("@MotherLastName", dto.MotherLastName);
                        command.Parameters.AddWithValue("@RelationshipWithChild", dto.RelationshipWithChild);
                        command.Parameters.AddWithValue("@NumberOfCopies", dto.NumberOfCopies);
                        command.Parameters.AddWithValue("@Purpose", dto.Purpose);

                        // Document Files
                        command.Parameters.AddWithValue("@DischargeDocument",
                            filePaths.TryGetValue("DischargeDocument", out string? dischargePath) ? dischargePath : DBNull.Value);
                        command.Parameters.AddWithValue("@IdProofDocument",
                            filePaths.TryGetValue("IdProofDocument", out string? idProofPath) ? idProofPath : DBNull.Value);
                        command.Parameters.AddWithValue("@AddressProofDocument",
                            filePaths.TryGetValue("AddressProofDocument", out string? addressProofPath) ? addressProofPath : DBNull.Value);
                        command.Parameters.AddWithValue("@AdditionalDocument",
                            filePaths.TryGetValue("AdditionalDocument", out string? additionalPath) ? additionalPath : DBNull.Value);

                        // Output parameter
                        var outputParam = new SqlParameter("@NewID", SqlDbType.Int) { Direction = ParameterDirection.Output };
                        command.Parameters.Add(outputParam);

                        await command.ExecuteNonQueryAsync();
                        newId = Convert.ToInt32(outputParam.Value);
                    }

                    // Get generated ApplicationID
                    using (var command = new SqlCommand("SELECT ApplicationID FROM Applications WHERE ID = @ID", connection))
                    {
                        command.Parameters.AddWithValue("@ID", newId);
                        var result = await command.ExecuteScalarAsync();
                        applicationId = result?.ToString() ?? string.Empty;
                    }
                }

                return (true, applicationId, "Birth certificate application created successfully");
            }
            catch (Exception ex)
            {
                return (false, string.Empty, ex.Message);
            }
        }
    }
}
