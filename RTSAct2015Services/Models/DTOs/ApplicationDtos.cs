namespace RTSAct2015Services.Models.DTOs
{
    // Response wrapper for API calls
    public class ResponseDto<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public string? ErrorCode { get; set; }
        public List<string>? Errors { get; set; }
    }

    // Application response DTO
    public class ApplicationResponseDto
    {
        public string Status { get; set; } = string.Empty;
        public string ApplicationID { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;

        // Additional properties to prevent errors from other forms
        public string? GUTComplaintID { get; set; }
        public string? ComplaintID { get; set; }
        public string? ReferenceNumber { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? FormType { get; set; }
    }
}
