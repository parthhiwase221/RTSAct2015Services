namespace RTSAct2015Services.Models.DTOs
{
    public class TrackApplicationResponseDto
    {
        public string ComplaintNumber { get; set; } = string.Empty;
        public string ApplicationType { get; set; } = string.Empty;
        public string FormName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? ResolvedDate { get; set; }
        public string ApplicantName { get; set; } = string.Empty;
        public string Mobile { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;
        public string Remarks { get; set; } = string.Empty;
        public string AssignedTo { get; set; } = string.Empty;
        public bool IsFound { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;

        // Status with Marathi translation
        public string StatusInMarathi => Status switch
        {
            "Submitted" => "सबमिट केले",
            "In Progress" => "प्रक्रिया सुरू",
            "Under Review" => "पुनरावलोकनाधीन",
            "Approved" => "मंजूर",
            "Rejected" => "नाकारले",
            "Completed" => "पूर्ण",
            _ => Status
        };

        // Priority with Marathi translation  
        public string PriorityInMarathi => Priority switch
        {
            "High" => "उच्च",
            "Medium" => "मध्यम",
            "Low" => "कमी",
            _ => Priority
        };
    }
}
