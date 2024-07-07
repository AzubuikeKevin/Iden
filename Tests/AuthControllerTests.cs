using Iden.AppDBContext;
using Iden.Controllers;
using Iden.DTOs;
using Iden.Entities;
using Iden.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace Iden.Tests
{
    
    public class AuthControllerTests
    {
        private readonly Mock<UserManager<User>> _mockUserManager;
        private readonly Mock<SignInManager<User>> _mockSignInManager;
        private readonly Mock<ITokenService> _mockTokenService;
        private readonly AppDbContext _context;
        private readonly AuthController _controller;
        public AuthControllerTests()
        {
            _mockUserManager = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), null, new PasswordHasher<User>(), null, null, null, null, null, null);

            _mockSignInManager = new Mock<SignInManager<User>>(
                _mockUserManager.Object,
                Mock.Of<IHttpContextAccessor>(),
                Mock.Of<IUserClaimsPrincipalFactory<User>>(),
                null, null, null, null);

            _mockTokenService = new Mock<ITokenService>();


            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;
            _context = new AppDbContext(options);

            _controller = new AuthController(_mockUserManager.Object, _context, _mockSignInManager.Object, _mockTokenService.Object);

        }

        [Fact]
        public async Task Register_RegisterUserSuccessfully()
        {
            var request = new UserRegistrationReqest
            {
                firstName = "Adara",
                lastName = "Funaya",
                email = "adara.funaya@example.com",
                password = "P@@ssword123!",
                phone = "1234567890"
            };

            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                 .ReturnsAsync(IdentityResult.Success);

            _mockUserManager.Setup(x => x.AddToRoleAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.Register(request);

            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            var responseObject = createdAtActionResult.Value;

            Assert.Equal("success", responseObject.GetType().GetProperty("status").GetValue(responseObject, null));
            Assert.Equal("Registration successful", responseObject.GetType().GetProperty("message").GetValue(responseObject, null));
        }

        [Fact]
        public async Task Register_ShouldCreateOrganisationWithDefaultName()
        {
            // Arrange
            var request = new UserRegistrationReqest
            {
                firstName = "Bright",
                lastName = "Chimezie",
                email = "bright.Chimezie@example.com",
                password = "Password123!",
                phone = "1234567890"
            };

            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<string>()))
                 .ReturnsAsync(IdentityResult.Success);

            _mockUserManager.Setup(x => x.AddToRoleAsync(It.IsAny<User>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _controller.Register(request);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            var responseObject = createdAtActionResult.Value;

            Assert.Equal("success", responseObject.GetType().GetProperty("status").GetValue(responseObject, null));
            Assert.Equal("Registration successful", responseObject.GetType().GetProperty("message").GetValue(responseObject, null));

            var organisation = _context.Organization.FirstOrDefault();
            Assert.NotNull(organisation);
            Assert.Equal("Bright's Organisation", organisation.name);
        }

        [Fact]
        public async Task Login_User_Successfully()
        {
            var request = new UserLoginRequest
            {
                Email = "adara.chidera@example.com",
                Password = "Pas__sword123!"
            };

            var user = new User
            {
                Email = request.Email,
                userId = "123",
                firstName = "Adara",
                lastName = "Chidera",
                phone = "1234567890"
            };

            _mockUserManager.Setup(x => x.Users).Returns(new List<User> { user }.AsQueryable());
            _mockSignInManager.Setup(x => x.CheckPasswordSignInAsync(It.IsAny<User>(), It.IsAny<string>(), false))
                .ReturnsAsync(SignInResult.Success);

            _mockTokenService.Setup(x => x.CreateToken(It.IsAny<User>())).Returns("fake-jwt-token");

            var result = await _controller.Login(request);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var responseObject = okResult.Value;

            Assert.Equal("success", responseObject.GetType().GetProperty("status").GetValue(responseObject, null));
            Assert.Equal("Authentication successful", responseObject.GetType().GetProperty("message").GetValue(responseObject, null));
        }



        [Fact]
        public async Task Required_Fields_Are_Missing()
        {
            _controller.ModelState.AddModelError("Email", "Required");

            var request = new UserRegistrationReqest
            {
                firstName = "Tunji",
                lastName = "Buari",
                password = "P00sword123!",
                phone = "1234567890"
            };

            var result = await _controller.Register(request);

            var unprocessableEntityResult = Assert.IsType<UnprocessableEntityObjectResult>(result);
            var responseObject = unprocessableEntityResult.Value;

            Assert.NotNull(responseObject);
        }

        [Fact]
        public async Task Duplicate_Email()
        {
            var request = new UserRegistrationReqest
            {
                firstName = "Gbgenga",
                lastName = "Moses",
                email = "Gbgenga.Moses@example.com",
                password = "_assword123!",
                phone = "1234567890"
            };

            _mockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(new User());

            // Act
            var result = await _controller.Register(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var responseObject = badRequestResult.Value;

            Assert.Equal("Bad request", responseObject.GetType().GetProperty("status").GetValue(responseObject, null));
            Assert.Equal("Email already in use", responseObject.GetType().GetProperty("message").GetValue(responseObject, null));
        }
    }

    
}
