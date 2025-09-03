namespace RTSAct2015Services.Models.Entities
{
    public class GatturComplaint : ApplicationBase
    {
        public string GUTComplaintID { get; set; } = string.Empty;
        public string ZoneType { get; set; } = string.Empty;

        // Gutter repair specific properties
        public string ComplaintType { get; set; } = string.Empty;
        public string UrgencyLevel { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public GatturComplaint()
        {
            ApplicationType = "GUT";
        }
    }
}
