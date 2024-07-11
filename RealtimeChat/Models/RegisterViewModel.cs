using System.ComponentModel.DataAnnotations;

namespace RealtimeChat.Models
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Username required")]
        public string Name { get; set; } = "";

        [Required(ErrorMessage = "Password required")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = "";


        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords don't match")]
        public string ConfirmPassword { get; set; } = "";
    }
}
