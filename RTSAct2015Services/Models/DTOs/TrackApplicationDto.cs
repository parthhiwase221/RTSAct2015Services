using System.ComponentModel.DataAnnotations;

namespace RTSAct2015Services.Models.DTOs
{
    public class TrackApplicationDto
    {
        [Required(ErrorMessage = "कृपया तक्रार क्रमांक प्रविष्ट करा / Please enter complaint number")]
        [Display(Name = "तक्रार क्रमांक / Complaint Number")]
        [StringLength(20, ErrorMessage = "तक्रार क्रमांक 20 अक्षरांपेक्षा जास्त असू शकत नाही")]
        public string ComplaintNumber { get; set; } = string.Empty;

        [Display(Name = "मोबाइल नंबर / Mobile Number")]
        [Phone(ErrorMessage = "कृपया वैध मोबाइल नंबर प्रविष्ट करा")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "मोबाइल नंबर 10 अंकांचा असावा")]
        public string? MobileNumber { get; set; }
    }
}
