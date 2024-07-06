using System.ComponentModel.DataAnnotations;

namespace Iden.DTOs
{
    public class UserLoginRequest
    {
        [Required(ErrorMessage ="Email is required!")]
        public string Email { get; set; }
        [Required(ErrorMessage ="Email is required!")]
        public string Password { get; set; }
    }
}
