using Iden.Entities;
using Iden.Service;
using System.IdentityModel.Tokens.Jwt;
using Xunit;

namespace Iden.Tests
{
    public class TokenServiceTests
    {
        private readonly TokenService _tokenService;
        private readonly User _testUser;

        public TokenServiceTests()
        {
            var inMemorySettings = new Dictionary<string, string> {
            {"JWT:SigningKey", "fakejjlihj654jkakkse6rvshj343njd3tak45detppetdkj5jke6lee9r0r0t-efsnsj04dsm12sjmznc75ddnk"},
            {"JWT:Issuer", "http://localhost"},
            {"JWT:Audience", "http://localhost"}
        };

            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            _tokenService = new TokenService(configuration);
            _testUser = new User
            {
                Email = "testuser@example.com",
                UserName = "testuser"
            };
        }

        [Fact]
        public void CreateToken_ContainCorrectUserDetails()
        {
            // Act
            var token = _tokenService.CreateToken(_testUser);

            // Assert
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            Assert.Contains(jwtToken.Claims, c => c.Type == JwtRegisteredClaimNames.Email && c.Value == _testUser.Email);
            Assert.Contains(jwtToken.Claims, c => c.Type == JwtRegisteredClaimNames.GivenName && c.Value == _testUser.UserName);
        }

        [Fact]
        public void CreateTokenExpireAtCorrectTime()
        {
            // Act
            var token = _tokenService.CreateToken(_testUser);

            // Assert
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            var expectedExpiration = DateTime.UtcNow.AddMinutes(7);
            var actualExpiration = jwtToken.ValidTo;

            Assert.True((expectedExpiration - actualExpiration).TotalSeconds < 1, "Token expiration time mismatch");
        }
    }
}
