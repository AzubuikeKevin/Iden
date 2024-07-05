using Iden.Entities;

namespace Iden.DTOs
{
    public class UserRegistrationResponse
    {
        public string AccessToken { get; set; }
        public UserResponse User { get; set; }
    }

    public class UserResponse
    {
        public string userId { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public string email { get; set; }
        public string phone { get; set; }
    }
}
