using System.ComponentModel.DataAnnotations;

namespace WebApplication1.UserRegisterModels
{
    public class RegisterModel
    {
        [Required(ErrorMessage ="UserName filed is required")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "password filed is required")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Email filed is required")]
        public string Email { get; set; }
    }
}
