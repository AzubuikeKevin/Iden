using Iden.Entities;

namespace Iden.DTOs
{
    public class UserLoginReponse
    {
        public string AccessToken { get; set; }
        public UserResponse User { get; set; }
    }
}
