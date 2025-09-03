using System.ComponentModel.DataAnnotations;

namespace RTSAct2015.Models.DTOs
{
    public class DeathCertificateCreateDto
    {
        // Deceased Person Details
        [Required]
        [StringLength(100)]
        public string DeceasedFullName { get; set; } = string.Empty;

        [Required]
        public DateTime DateOfDeath { get; set; }

        [Required]
        public TimeSpan TimeOfDeath { get; set; }

        [Required]
        public string Gender { get; set; } = string.Empty;

        [Required]
        [Range(0, 150)]
        public int AgeAtDeath { get; set; }

        [Required]
        [StringLength(100)]
        public string PlaceOfDeath { get; set; } = string.Empty;

        [Required]
        public string CauseOfDeath { get; set; } = string.Empty;

        [StringLength(100)]
        public string? HospitalName { get; set; }

        // Applicant Details
        [Required]
        public string ApplicantTitle { get; set; } = string.Empty;

        [Required]
        [StringLength(40)]
        public string ApplicantFirstName { get; set; } = string.Empty;

        [StringLength(40)]
        public string? ApplicantMiddleName { get; set; }

        [Required]
        [StringLength(40)]
        public string ApplicantLastName { get; set; } = string.Empty;

        [Required]
        public string RelationshipWithDeceased { get; set; } = string.Empty;

        [Required]
        [StringLength(15)]
        public string Mobile { get; set; } = string.Empty;

        [Required]
        [StringLength(10)]
        public string PinCode { get; set; } = string.Empty;

        // Address Details
        [Required]
        [StringLength(100)]
        public string Street { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Area { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string City { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string District { get; set; } = string.Empty;

        // Certificate Requirements
        [Required]
        public int NumberOfCopies { get; set; }

        [Required]
        public string Purpose { get; set; } = string.Empty;

        // Document Files
        public IFormFile? MedicalCertificate { get; set; }
        public IFormFile? IdProofDocument { get; set; }
        public IFormFile? AddressProofDocument { get; set; }
        public IFormFile? AdditionalDocument { get; set; }

        public decimal Latitude { get; set; } = 0;
        public decimal Longitude { get; set; } = 0;
    }
}
