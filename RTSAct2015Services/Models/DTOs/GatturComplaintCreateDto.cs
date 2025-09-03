using System.ComponentModel.DataAnnotations;

namespace RTSAct2015Services.Models.DTOs
{
    public class GatturComplaintCreateDto
    {
        [Required(ErrorMessage = "शीर्षक आवश्यक आहे")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "पहिले नाव आवश्यक आहे")]
        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters")]
        public string FirstName { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "Middle name cannot exceed 50 characters")]
        public string? MiddleName { get; set; }

        [Required(ErrorMessage = "आडनाव आवश्यक आहे")]
        [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "मोबाईल क्रमांक आवश्यक आहे")]
        [RegularExpression(@"^[6-9]\d{9}$", ErrorMessage = "वैध 10 अंकी मोबाईल क्रमांक टाका")]
        public string Mobile { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "वैध ईमेल टाका")]
        [StringLength(80, ErrorMessage = "Email cannot exceed 80 characters")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "प्रभाग निवडा")]
        public string ZoneType { get; set; } = string.Empty;

        [Required(ErrorMessage = "रस्त्याचे नाव आवश्यक आहे")]
        [StringLength(100, ErrorMessage = "Street cannot exceed 100 characters")]
        public string Street { get; set; } = string.Empty;

        [Required(ErrorMessage = "क्षेत्राचे नाव आवश्यक आहे")]
        [StringLength(100, ErrorMessage = "Area cannot exceed 100 characters")]
        public string Area { get; set; } = string.Empty;

        [Required(ErrorMessage = "शहराचे नाव आवश्यक आहे")]
        [StringLength(50, ErrorMessage = "City cannot exceed 50 characters")]
        public string City { get; set; } = string.Empty;

        [Required(ErrorMessage = "पिन कोड आवश्यक आहे")]
        [RegularExpression(@"^[1-9][0-9]{5}$", ErrorMessage = "वैध 6 अंकी पिन कोड टाका")]
        public string PinCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "ओळखचिन्ह आवश्यक आहे")]
        [StringLength(100, ErrorMessage = "Landmark cannot exceed 100 characters")]
        public string Landmark { get; set; } = string.Empty;

        [Required(ErrorMessage = "अक्षांश आवश्यक आहे")]
        [Range(-90, 90, ErrorMessage = "अक्षांश -90 ते 90 मध्ये असावा")]
        public decimal Latitude { get; set; } = 18.5204M;

        [Required(ErrorMessage = "रेखांश आवश्यक आहे")]
        [Range(-180, 180, ErrorMessage = "रेखांश -180 ते 180 मध्ये असावा")]
        public decimal Longitude { get; set; } = 73.8567M;

        // Document Properties
        public string? DocumentPath { get; set; }
        public string? DocumentName { get; set; }
        public string? DocumentType { get; set; }
        public long? DocumentSize { get; set; }

        public IFormFile? DocumentFile { get; set; }
    }
}
