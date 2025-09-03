using System.ComponentModel.DataAnnotations;

namespace RTSAct2015Services.Models.DTOs
{
    public class TreeFellingCreateDto
    {
        // Personal Details
        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "First name is required")]
        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters")]
        public string FirstName { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "Middle name cannot exceed 50 characters")]
        public string? MiddleName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mobile is required")]
        [RegularExpression(@"^[6-9]\d{9}$", ErrorMessage = "Please enter a valid 10-digit mobile number")]
        public string Mobile { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Invalid email address")]
        [StringLength(80, ErrorMessage = "Email cannot exceed 80 characters")]
        public string? Email { get; set; }

        // Address Details
        [Required(ErrorMessage = "Street is required")]
        [StringLength(100, ErrorMessage = "Street cannot exceed 100 characters")]
        public string Street { get; set; } = string.Empty;

        [Required(ErrorMessage = "Area is required")]
        [StringLength(100, ErrorMessage = "Area cannot exceed 100 characters")]
        public string Area { get; set; } = string.Empty;

        [Required(ErrorMessage = "City is required")]
        [StringLength(50, ErrorMessage = "City cannot exceed 50 characters")]
        public string City { get; set; } = string.Empty;

        [Required(ErrorMessage = "Pin code is required")]
        [RegularExpression(@"^[1-9][0-9]{5}$", ErrorMessage = "Please enter a valid 6-digit pin code")]
        public string PinCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Landmark is required")]
        [StringLength(100, ErrorMessage = "Landmark cannot exceed 100 characters")]
        public string Landmark { get; set; } = string.Empty;

        // Location
        [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90")]
        public decimal Latitude { get; set; } = 19.8762M;

        [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180")]
        public decimal Longitude { get; set; } = 75.3433M;

        // Document Properties
        public string? DocumentPath { get; set; }
        public string? DocumentName { get; set; }
        public string? DocumentType { get; set; }
        public long? DocumentSize { get; set; }

        // File Path Properties
        public string? PropertyTaxReceiptFilePath { get; set; }
        public string? TreePhotographFilePath { get; set; }
        public string? AadhaarCardFilePath { get; set; }
        public string? BuildingPermissionFilePath { get; set; }
        public string? SanctionedPlanFilePath { get; set; }
        public string? FormCDFilePath { get; set; }

        // Tree Felling specific properties
        [Required(ErrorMessage = "Reason for cutting is required")]
        public string ReasonForFelling { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tree type is required")]
        public string TreeType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Owner type is required")]
        public string OwnerType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Type of applicant is required")]
        public string TypeOfApplicant { get; set; } = string.Empty;

        [Required(ErrorMessage = "Number of tree felling is required")]
        [Range(1, 100, ErrorMessage = "Tree count must be between 1 and 100")]
        public int NoOfTreeFelling { get; set; }

        public string? TreeSpecies { get; set; }
        public string? OtherReason { get; set; }

        // File Upload Properties
        public IFormFile? PropertyTaxReceiptFile { get; set; }
        public IFormFile? TreePhotographFile { get; set; }
        public IFormFile? AadhaarCardFile { get; set; }
        public IFormFile? BuildingPermissionFile { get; set; }
        public IFormFile? SanctionedPlanFile { get; set; }
        public IFormFile? FormCDFile { get; set; }
        public IFormFile? DocumentFile { get; set; }

        // **REMOVED VALIDATION ATTRIBUTES - LET CONTROLLER HANDLE IT**
        public bool TermsCondition1 { get; set; } = false;
        public bool TermsCondition2 { get; set; } = false;
        public bool TermsCondition3 { get; set; } = false;
        public bool TermsCondition4 { get; set; } = false;
        public bool SelectAllTerms { get; set; } = false;
    }
}
