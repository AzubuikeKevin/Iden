using Iden.AppDBContext;
using Iden.DTOs;
using Iden.Entities;
using Iden.Helper;
using Iden.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.Serialization;

namespace Iden.Controllers
{
    [Route("auth/")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly AppDbContext _context;
        private readonly SignInManager<User> _signInManager;
        private readonly ITokenService _tokenService;

        public AuthController(UserManager<User> userManager,
                              AppDbContext context,
                              SignInManager<User> signInManager,
                              ITokenService tokenService)
        {
            _userManager = userManager;
            _context = context;
            _signInManager = signInManager;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationReqest request)
        {
            if (!ModelState.IsValid)
            {
                var validationErrors = ValidationHelper.GetValidationErrors(ModelState, typeof(UserRegistrationReqest));

                return UnprocessableEntity(new
                {
                    errors = validationErrors
                });
            }

            try
            {
                var existingUser = await _userManager.FindByEmailAsync(request.email);
                if (existingUser != null)
                {
                    return BadRequest(new
                    {
                        status = "Bad request",
                        message = "Email already in use",
                        statusCode = 400
                    });
                }
                string randomText = StringHelper.RandomString(2);

                var appUser = new User
                {
                    userId = Guid.NewGuid().ToString(),
                    UserName = $"{request.firstName}{request.lastName}{randomText}",
                    email = request.email,
                    firstName = request.firstName,
                    lastName = request.lastName,
                    Email = request.email,
                    password = "to-be-updated",
                    phone = request.phone

                };

                var createdUser = await _userManager.CreateAsync(appUser, request.password);
                if (createdUser.Succeeded)
                {
                    //Ensure password is hashed
                    appUser.password = _userManager.PasswordHasher.HashPassword(appUser, request.password);
                    await _userManager.UpdateAsync(appUser);

                    var roleResult = await _userManager.AddToRoleAsync(appUser, "User");
                    if (roleResult.Succeeded)
                    {
                        var organisation = new Organisation
                        {
                            orgId = Guid.NewGuid().ToString(),
                            name = $"{request.firstName}'s Organisation",
                            description = "Auto-generated organisation",
                            UserOrganisations =
                            [
                                new UserOrganisation
                                {
                                    User = appUser
                                }
                            ]
                        };

                        _context.Organization.Add(organisation);
                        await _context.SaveChangesAsync();

                        var response = new UserRegistrationResponse
                        {
                            AccessToken = _tokenService.CreateToken(appUser),
                            User = new UserResponse
                            {
                                userId = appUser.userId,
                                firstName = appUser.firstName,
                                lastName = appUser.lastName,
                                email = appUser.Email,
                                phone = appUser.phone
                            }
                        };

                        return CreatedAtAction(nameof(Register), new
                        {
                            status = "success",
                            message = "Registration successful",
                            data = response
                        });
                    }
                    else
                    {
                        return BadRequest(new
                        {
                            status = "Bad request",
                            message = "Registration unsuccessful",
                            statusCode = 400
                        });
                    }
                }
                else
                {
                    return StatusCode(500, createdUser.Errors);
                }
            }
            catch (Exception e)
            {

                return StatusCode(500, e.Message);
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]UserLoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                var validationErrors = ValidationHelper.GetValidationErrors(ModelState, typeof(UserLoginRequest));

                return UnprocessableEntity(new
                {
                    errors = validationErrors
                });
            }

            var user = _userManager.Users.FirstOrDefault(x => x.Email == request.Email.ToLower());
            if (user == null)
            {
                return Unauthorized("User not found!");
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (!result.Succeeded)
            {
                return Unauthorized((new
                {
                    status = "Bad request",
                    message = "Authentication failed",
                    statusCode = 401
                }));
            }

            var response = new UserLoginReponse
            {
                AccessToken = _tokenService.CreateToken(user),
                User = new UserResponse
                {
                    userId = user.userId,
                    firstName = user.firstName,
                    lastName = user.lastName,
                    email = user.Email,
                    phone = user.phone
                }
            };

            return Ok(new
            {
                status = "success",
                message = "Authentication successful",
                data = response
            });
        }
    }
}
