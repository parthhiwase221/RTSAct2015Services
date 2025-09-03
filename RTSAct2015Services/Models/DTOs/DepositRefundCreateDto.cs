using System.ComponentModel.DataAnnotations;

namespace RTSAct2015Services.Models.DTOs
{
    public class DepositRefundCreateDto
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
        [RegularExpression(@"^[6-9]\d{9}$", ErrorMessage = "Please enter a valid 10-digit mobile number starting with 6-9")]
        public string Mobile { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Invalid email address")]
        [StringLength(80, ErrorMessage = "Email cannot exceed 80 characters")]
        public string? Email { get; set; }

        // Address Details
        [StringLength(200, ErrorMessage = "Applicant address cannot exceed 200 characters")]
        public string? ApplicantAddress { get; set; }

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
        [RegularExpression(@"^[1-9][0-9]{5}$", ErrorMessage = "Please enter a valid 6-digit pin code not starting with 0")]
        public string PinCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Landmark is required")]
        [StringLength(100, ErrorMessage = "Landmark cannot exceed 100 characters")]
        public string Landmark { get; set; } = string.Empty;

        // Location
        [Range(-90, 90, ErrorMessage = "Latitude must be between -90 and 90")]
        public decimal Latitude { get; set; } = 19.8762M;

        [Range(-180, 180, ErrorMessage = "Longitude must be between -180 and 180")]
        public decimal Longitude { get; set; } = 75.3433M;

        // **UPDATED: Document Properties for file storage**
        public string? DocumentPath { get; set; }
        public string? DocumentName { get; set; }
        public string? DocumentType { get; set; }
        public long? DocumentSize { get; set; }

        // **NEW: File Path Properties - These store the saved file paths**
        public string? OccupancyCertFilePath { get; set; }
        public string? BuildingDepReceiptFilePath { get; set; }
        public string? TreeDepReceiptFilePath { get; set; }
        public string? PropertyTaxReceiptFilePath { get; set; }
        public string? BuildingPermissionCertFilePath { get; set; }
        public string? ChallanFilePath { get; set; }
        public string? NOCFilePath { get; set; }

        // Deposit Refund Specific Fields
        [Required(ErrorMessage = "Applicant type is required")]
        public string ApplicantType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Zone name is required")]
        public string ZoneName { get; set; } = string.Empty;

        // Property Details
        [Required(ErrorMessage = "Gut number is required")]
        [StringLength(20, ErrorMessage = "Gut number cannot exceed 20 characters")]
        public string GutNo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Survey number is required")]
        [StringLength(20, ErrorMessage = "Survey number cannot exceed 20 characters")]
        public string SurveyNo { get; set; } = string.Empty;

        [Required(ErrorMessage = "CTS number is required")]
        [StringLength(20, ErrorMessage = "CTS number cannot exceed 20 characters")]
        public string CtsNo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Occupancy certificate number is required")]
        [StringLength(100, ErrorMessage = "Occupancy certificate number cannot exceed 100 characters")]
        public string OccupancyCertificateNo { get; set; } = string.Empty;

        [Required(ErrorMessage = "Property address is required")]
        [StringLength(300, ErrorMessage = "Property address cannot exceed 300 characters")]
        public string PropertyAddress { get; set; } = string.Empty;

        // Deposit Information
        [StringLength(200, ErrorMessage = "Reason for deposit cannot exceed 200 characters")]
        public string? ReasonForDeposit { get; set; }

        [StringLength(50, ErrorMessage = "Building permit file number cannot exceed 50 characters")]
        public string? BuildingPermitFileNo { get; set; }

        [Range(0, 999999.99, ErrorMessage = "Building deposit amount must be between 0 and 999999.99")]
        public decimal? BuildingDepositAmount { get; set; }

        [Range(0, 999999.99, ErrorMessage = "Tree deposit amount must be between 0 and 999999.99")]
        public decimal? TreeDepositAmount { get; set; }

        // Bank Details
        [StringLength(20, ErrorMessage = "Bank account number cannot exceed 20 characters")]
        public string? BankAccountNumber { get; set; }

        [StringLength(100, ErrorMessage = "Bank name cannot exceed 100 characters")]
        public string? BankName { get; set; }

        [RegularExpression(@"^[A-Z]{4}0[A-Z0-9]{6}$", ErrorMessage = "Please enter a valid IFSC code (e.g., SBIN0001234)")]
        public string? IFSCCode { get; set; }

        // Challan Details
        [StringLength(50, ErrorMessage = "Challan number cannot exceed 50 characters")]
        public string? ChallanNo { get; set; }

        [Range(0, 999999.99, ErrorMessage = "Challan amount must be between 0 and 999999.99")]
        public decimal? ChallanAmount { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ChallanPaidDate { get; set; }

        // NOC and Certificate Status
        public string? HasGardenNOC { get; set; }
        public string? OccupancyCertificateStatus { get; set; }

        // Deposit Type Selection (Fixed Boolean Handling)
        public bool RefundBuildingDeposit { get; set; } = false;
        public bool RefundTreeDeposit { get; set; } = false;

        // **File Upload Properties - These receive the uploaded files from form**
        public IFormFile? OccupancyCertFile { get; set; }
        public IFormFile? BuildingDepReceiptFile { get; set; }
        public IFormFile? TreeDepReceiptFile { get; set; }
        public IFormFile? PropertyTaxReceiptFile { get; set; }
        public IFormFile? BuildingPermissionCertFile { get; set; }
        public IFormFile? ChallanFile { get; set; }
        public IFormFile? NOCFile { get; set; }
        public IFormFile? DocumentFile { get; set; }

        // Terms and Conditions (Fixed Boolean Handling)
        [Required(ErrorMessage = "Declaration is required")]
        public bool Declaration { get; set; } = false;

        public bool? SelectAll { get; set; }
        public string? Notes { get; set; }
        public string? Remarks { get; set; }
        public string? TermsAccepted { get; set; }
    }
}
