namespace RTSAct2015Services.Models.Entities
{
    public class ApplicationTrackingEntity
    {
        public int ID { get; set; }
        public string ApplicationID { get; set; } = string.Empty;
        public string ApplicationType { get; set; } = string.Empty;
        public string FormName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? ResolvedDate { get; set; }
        public string Title { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string MiddleName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Mobile { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;
        public string Street { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Remarks { get; set; } = string.Empty;
        public string AssignedTo { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
