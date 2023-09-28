using System.ComponentModel.DataAnnotations;

namespace WebApplication1.UserRegisterModels
{
    public class Login
    {
        [Required]
        public string UserName { get; set; }
        [Required]
        public string Password { get; set; }

    }
}
