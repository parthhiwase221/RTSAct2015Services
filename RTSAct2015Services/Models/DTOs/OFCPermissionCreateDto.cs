using System.ComponentModel.DataAnnotations;

namespace RTSAct2015Services.Models.DTOs
{
    public class OFCPermissionCreateDto
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
        [Required(ErrorMessage = "Latitude is required")]
        [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90")]
        public decimal Latitude { get; set; } = 18.5204M;

        [Required(ErrorMessage = "Longitude is required")]
        [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180")]
        public decimal Longitude { get; set; } = 73.8567M;

        // Document Properties
        public string? DocumentPath { get; set; }
        public string? DocumentName { get; set; }
        public string? DocumentType { get; set; }
        public long? DocumentSize { get; set; }

        // OFC specific properties
        [Required(ErrorMessage = "Installation type is required")]
        public string InstallationType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Cable type is required")]
        public string CableType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Company name is required")]
        [StringLength(200, ErrorMessage = "Company name cannot exceed 200 characters")]
        public string CompanyName { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "Company registration number cannot exceed 100 characters")]
        public string? CompanyRegNo { get; set; }

        [StringLength(100, ErrorMessage = "Authorized representative name cannot exceed 100 characters")]
        public string? AuthorizedRep { get; set; }

        [Required(ErrorMessage = "Cable length is required")]
        [Range(0.1, 10000, ErrorMessage = "Length must be between 0.1 and 10000 meters")]
        public decimal TotalLength { get; set; }

        [Range(0.1, 50, ErrorMessage = "Trench width must be between 0.1 and 50 meters")]
        public decimal? TrenchWidth { get; set; }

        [StringLength(100, ErrorMessage = "Place name cannot exceed 100 characters")]
        public string? PlaceName { get; set; }

        [StringLength(50, ErrorMessage = "Work type cannot exceed 50 characters")]
        public string? WorkType { get; set; }

     
        // File Upload Properties for form files
        public IFormFile? DocumentFile { get; set; }
    }
}
