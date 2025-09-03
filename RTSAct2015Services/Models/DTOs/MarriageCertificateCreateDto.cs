using System.ComponentModel.DataAnnotations;

namespace RTSAct2015.Models.DTOs
{
    public class MarriageCertificateCreateDto
    {
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
        [StringLength(200)]
        public string ApplicantAddress { get; set; } = string.Empty;

        [Required]
        [StringLength(15)]
        public string Mobile { get; set; } = string.Empty;

        [StringLength(80)]
        public string? Email { get; set; }

        // Application Details
        [Required]
        public string Zone { get; set; } = string.Empty;

        [Required]
        public DateTime MarriageDate { get; set; }

        [Required]
        public string PersonalLaw { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string PlaceOfMarriage { get; set; } = string.Empty;

        // Groom Details
        [Required]
        [StringLength(100)]
        public string GroomName { get; set; } = string.Empty;

        [StringLength(100)]
        public string? GroomAlternateName { get; set; }

        [Required]
        public string GroomReligionBirth { get; set; } = string.Empty;

        [Required]
        public DateTime GroomBirthDate { get; set; }

        [Required]
        [Range(21, 100)]
        public int GroomAge { get; set; }

        [Required]
        public string GroomMaritalStatus { get; set; } = string.Empty;

        [Required]
        public string GroomOccupation { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string GroomAddress { get; set; } = string.Empty;

        // Bride Details
        [Required]
        [StringLength(100)]
        public string BrideName { get; set; } = string.Empty;

        [StringLength(100)]
        public string? BrideAlternateName { get; set; }

        [Required]
        public string BrideReligionBirth { get; set; } = string.Empty;

        [Required]
        public DateTime BrideBirthDate { get; set; }

        [Required]
        [Range(18, 100)]
        public int BrideAge { get; set; }

        [Required]
        public string BrideMaritalStatus { get; set; } = string.Empty;

        [Required]
        public string BrideOccupation { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string BrideAddress { get; set; } = string.Empty;

        // Priest Details
        [Required]
        [StringLength(100)]
        public string PriestName { get; set; } = string.Empty;

        [Required]
        public string PriestReligion { get; set; } = string.Empty;

        [Required]
        [Range(21, 100)]
        public int PriestAge { get; set; }

        [Required]
        [StringLength(200)]
        public string PriestAddress { get; set; } = string.Empty;

        // Document Files
        public IFormFile? GroomPhoto { get; set; }
        public IFormFile? BridePhoto { get; set; }
        public IFormFile? PriestPhoto { get; set; }
        public IFormFile? MarriageCard { get; set; }
        public IFormFile? GroupPhoto { get; set; }

        public decimal Latitude { get; set; } = 0;
        public decimal Longitude { get; set; } = 0;
    }
}
