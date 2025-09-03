using System.ComponentModel.DataAnnotations;

namespace RTSAct2015.Models.DTOs
{
    public class BirthCertificateCreateDto
    {
        // Child Details
        [Required]
        [StringLength(100)]
        public string ChildName { get; set; } = string.Empty;

        [Required]
        public string Gender { get; set; } = string.Empty;

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        public string PlaceOfBirth { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string HospitalName { get; set; } = string.Empty;

        // Parent Details
        [Required]
        public string FatherTitle { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string FatherFirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string FatherLastName { get; set; } = string.Empty;

        [Required]
        public string MotherTitle { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string MotherFirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string MotherLastName { get; set; } = string.Empty;

        // Applicant Details
        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        [StringLength(40)]
        public string FirstName { get; set; } = string.Empty;

        [StringLength(40)]
        public string? MiddleName { get; set; }

        [Required]
        [StringLength(40)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        public string RelationshipWithChild { get; set; } = string.Empty;

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

        [StringLength(100)]
        public string? Landmark { get; set; }

        [Required]
        public string Purpose { get; set; } = string.Empty;

        // Document Files (All nullable)
        public IFormFile? DischargeDocument { get; set; }
        public IFormFile? IdProofDocument { get; set; }
        public IFormFile? AddressProofDocument { get; set; }
        public IFormFile? AdditionalDocument { get; set; }

        public decimal Latitude { get; set; } = 0;
        public decimal Longitude { get; set; } = 0;
    }
}
