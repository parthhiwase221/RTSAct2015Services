namespace RTSAct2015Services.Models.Entities
{
    public class PotholeComplaint : ApplicationBase
    {
        public string RoadName { get; set; } = string.Empty;
        public string PotholeSize { get; set; } = string.Empty;
        public string TrafficImpact { get; set; } = string.Empty;
        public string RoadType { get; set; } = string.Empty;
        public int PotholeCount { get; set; }

        public PotholeComplaint()
        {
            ApplicationType = "POTHOLE";
        }
    }
}
