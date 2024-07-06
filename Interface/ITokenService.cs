using Iden.Entities;

namespace Iden.Interface
{
    public interface ITokenService
    {
        string CreateToken(User user);
    }
}
