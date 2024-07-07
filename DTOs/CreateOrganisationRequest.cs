using System.ComponentModel.DataAnnotations;

namespace Iden.DTOs
{
    public class CreateOrganisationRequest
    {
        [Required(ErrorMessage = "Name is required.")]
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
