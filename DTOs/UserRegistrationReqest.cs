using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Iden.DTOs
{
    public class UserRegistrationReqest
    {
        [Required(ErrorMessage = "First name is required.")]
        public string firstName { get; set; }
        [Required(ErrorMessage = "Last name is required.")]
        public string lastName { get; set; }
        [Required(ErrorMessage = "Email is required.")]
        public string email { get; set; }
        [Required(ErrorMessage = "Password is required.")]
        public string password { get; set; }
        public string phone { get; set; }

    }
}
