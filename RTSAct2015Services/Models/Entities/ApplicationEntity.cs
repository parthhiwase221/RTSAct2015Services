namespace RTSAct2015Services.Models.Entities
{
    public class ApplicationEntity
    {
        public int ID { get; set; }
        public string ApplicationID { get; set; } = string.Empty;
        public string ApplicationType { get; set; } = string.Empty;
        public string? FormName { get; set; }
        public string Status { get; set; } = "Submitted";
        public string Priority { get; set; } = "Medium";
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? ResolvedDate { get; set; }
        public bool IsActive { get; set; } = true;
        public string? AssignedTo { get; set; }
        public string? Remarks { get; set; }

        // Personal Details
        public string Title { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public string LastName { get; set; } = string.Empty;
        public string Mobile { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? ApplicantAddress { get; set; }
        public string? FlatNo { get; set; }
        public string? Colony { get; set; }
        public string? ZoneType { get; set; }
        public string Street { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string PinCode { get; set; } = string.Empty;
        public string Landmark { get; set; } = string.Empty;
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }

        // Document Properties
        public string? DocumentPath { get; set; }
        public string? DocumentName { get; set; }
        public string? DocumentType { get; set; }
        public long? DocumentSize { get; set; }
        public string? DocumentFile { get; set; }

        // Deposit Refund Properties
        public string? ApplicantType { get; set; }
        public string? Zone { get; set; }
        public string? GutNo { get; set; }
        public string? SurveyNo { get; set; }
        public string? CtsNo { get; set; }
        public string? OccupancyCertificateNo { get; set; }
        public string? PropertyAddress { get; set; }
        public string? DepositReason { get; set; }
        public string? PermitFileNo { get; set; }
        public decimal? BuildingDepositAmount { get; set; }
        public decimal? TreeDepositAmount { get; set; }
        public string? BankAccountNumber { get; set; }
        public string? BankName { get; set; }
        public string? IFSCCode { get; set; }
        public string? ChallanNo { get; set; }
        public decimal? ChallanAmount { get; set; }
        public DateTime? ChallanPaidDate { get; set; }
        public string? HasGardenNOC { get; set; }
        public string? OccupancyCertificateStatus { get; set; }
        public bool? RefundBuildingDeposit { get; set; }
        public bool? RefundTreeDeposit { get; set; }
        public bool? Declaration { get; set; }

        // File Paths
        public string? PropertyTaxFile { get; set; }
        public string? TreePhotoFile { get; set; }
        public string? AadhaarFile { get; set; }
        public string? BuildingPermitFile { get; set; }
        public string? ApprovedPlanFile { get; set; }
        public string? FormCDFile { get; set; }
        public string? ChallanFile { get; set; }
        public string? OccupancyCertFile { get; set; }
        public string? BuildingDepReceiptFile { get; set; }
        public string? TreeDepReceiptFile { get; set; }
        public string? PropertyTaxReceiptFile { get; set; }
        public string? BuildingPermissionCertFile { get; set; }
        public string? NOCFile { get; set; }
        public string? BuildingDepositReceiptFile { get; set; }
        public string? TreeDepositReceiptFile { get; set; }

        // Additional Fields
        public string? TermsAccepted { get; set; }
        public bool? SelectAllTerms { get; set; }
        public string? Notes { get; set; }
        public bool? SelectAll { get; set; }

        // Computed Properties
        public string FullName => $"{Title} {FirstName} {MiddleName} {LastName}".Replace("  ", " ").Trim();
        public string FullAddress => $"{Street}, {Area}, {City} - {PinCode}";
    }
}
