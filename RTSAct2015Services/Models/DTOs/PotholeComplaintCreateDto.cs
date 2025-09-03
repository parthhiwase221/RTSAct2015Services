using System.ComponentModel.DataAnnotations;

namespace RTSAct2015Services.Models.DTOs
{
    public class PotholeComplaintCreateDto
    {
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

        [Required(ErrorMessage = "Latitude is required")]
        [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90")]
        public decimal Latitude { get; set; } = 18.5204M;

        [Required(ErrorMessage = "Longitude is required")]
        [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180")]
        public decimal Longitude { get; set; } = 73.8567M;

        [Required(ErrorMessage = "Road name is required")]
        [StringLength(100, ErrorMessage = "Road name cannot exceed 100 characters")]
        public string RoadName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Pothole size is required")]
        public string PotholeSize { get; set; } = string.Empty;

        [Required(ErrorMessage = "Traffic impact is required")]
        public string TrafficImpact { get; set; } = string.Empty;

        [Required(ErrorMessage = "Road type is required")]
        public string RoadType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Pothole count is required")]
        [Range(1, 999, ErrorMessage = "Pothole count must be between 1 and 999")]
        public int PotholeCount { get; set; }

        // Document Properties
        public string? DocumentPath { get; set; }
        public string? DocumentName { get; set; }
        public string? DocumentType { get; set; }
        public long? DocumentSize { get; set; }

        public IFormFile? DocumentFile { get; set; }
    }
}
