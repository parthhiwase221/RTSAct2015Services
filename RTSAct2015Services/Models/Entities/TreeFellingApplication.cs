namespace RTSAct2015Services.Models.Entities
{
    public class TreeFellingApplication : ApplicationBase
    {
        // Tree Felling specific properties
        public string ReasonForFelling { get; set; } = string.Empty;
        public string TreeType { get; set; } = string.Empty;
        public string OwnerType { get; set; } = string.Empty;
        public int TreeCount { get; set; }
        public string TreeSpecies { get; set; } = string.Empty;
        public bool IsReplacementRequired { get; set; }

        public TreeFellingApplication()
        {
            ApplicationType = "TFL";
        }
    }
}
