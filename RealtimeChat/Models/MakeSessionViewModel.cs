

using System.ComponentModel.DataAnnotations;

namespace RealtimeChat.Models
{
    public class MakeSessionViewModel
    {
        [Required(ErrorMessage = "No name provided")]
        public string SessionName { get; set; } = "";
        [Required(ErrorMessage = "No user names provided"), MinLength(1, ErrorMessage = "Atleast one user required")]
        public string[] UserNames { get; set; } = [];
    }
}
