namespace Iden.Entities
{
    public class UserOrganisation
    {
        public string userId { get; set; }
        public User User { get; set; }
        public string orgId { get; set; }
        public Organisation Organisation { get; set; }
    }
}
