using System.ComponentModel.DataAnnotations;

namespace Iden.Entities
{
    public class Organisation
    {
        [Key]
        [Required]
        public string orgId { get; set; }
        [Required]
        public string name { get; set; }
        public string description { get; set; }

        public ICollection<UserOrganisation> UserOrganisations { get; set; }
    }
}
