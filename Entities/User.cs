using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Iden.Entities
{
    public class User : IdentityUser
    {
        [Key]
        [Required]
        public string userId { get; set; }
        [Required]
        public string firstName { get; set; }
        [Required]
        public string lastName { get; set; }
        [Required]
        [EmailAddress]
        [JsonIgnore]
        public string email { get; set; }
        [Required]
        public string password { get; set; }
        public string phone { get; set; }
        public ICollection<UserOrganisation> UserOrganisations { get; set; }

    }
}