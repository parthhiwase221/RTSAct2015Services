using System.ComponentModel.DataAnnotations;

namespace RTSAct2015Services.Models.Entities
{
    public abstract class ApplicationBase
    {
        public string ApplicationID { get; set; } = string.Empty;
        public string ApplicationType { get; set; } = string.Empty;

        [Required]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string FirstName { get; set; } = string.Empty;

        public string? MiddleName { get; set; }

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required]
        public string Mobile { get; set; } = string.Empty;

        public string? Email { get; set; }

        public string? FlatNo { get; set; }
        public string? Colony { get; set; }

        [Required]
        public string Street { get; set; } = string.Empty;

        [Required]
        public string Area { get; set; } = string.Empty;

        [Required]
        public string City { get; set; } = string.Empty;

        [Required]
        public string PinCode { get; set; } = string.Empty;

        [Required]
        public string Landmark { get; set; } = string.Empty;

        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }

        public string? DocumentPath { get; set; }
        public string? DocumentName { get; set; }
        public string? DocumentType { get; set; }
        public long? DocumentSize { get; set; }

        public string Status { get; set; } = "Submitted";
        public string Priority { get; set; } = "Medium";
        public string? AssignedTo { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? UpdatedDate { get; set; }
        public DateTime? ResolvedDate { get; set; }

        public string? Remarks { get; set; }
        public bool IsActive { get; set; } = true;

        // Computed Properties
        public string FullName => $"{Title} {FirstName} {MiddleName} {LastName}".Replace("  ", " ").Trim();
        public string FullAddress => $"{Street}, {Area}, {City} - {PinCode}";
    }
}
