using System.ComponentModel.DataAnnotations;

namespace Iden.DTOs
{
    public class AddUserToOrganisationRequest
    {
        [Required(ErrorMessage = "UserId is required.")]
        public string userId { get; set; }
    }
}
