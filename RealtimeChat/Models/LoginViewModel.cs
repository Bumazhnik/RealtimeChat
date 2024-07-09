using System.ComponentModel.DataAnnotations;

namespace RealtimeChat.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "No name provided")]
        public string Name { get; set; }

        [Required(ErrorMessage = "No password provided")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
