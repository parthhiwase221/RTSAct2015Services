namespace RTSAct2015Services.Models.Entities
{
    public class TreeTrimmingApplication : ApplicationBase
    {
        // Tree Trimming specific properties
        public string ReasonForTrimming { get; set; } = string.Empty;
        public string TreeType { get; set; } = string.Empty;
        public string OwnerType { get; set; } = string.Empty;
        public int TreeCount { get; set; }
        public string TreeSpecies { get; set; } = string.Empty;

        public TreeTrimmingApplication()
        {
            ApplicationType = "TTR";
        }
    }
}
