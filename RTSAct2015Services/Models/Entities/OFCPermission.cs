namespace RTSAct2015Services.Models.Entities
{
    public class OFCPermission : ApplicationBase
    {
        public string InstallationType { get; set; } = string.Empty;
        public string CableType { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public decimal Length { get; set; }

        public OFCPermission()
        {
            ApplicationType = "OFC";
        }
    }
}
