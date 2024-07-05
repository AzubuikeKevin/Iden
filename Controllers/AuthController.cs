using Iden.AppDBContext;
using Iden.DTOs;
using Iden.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Iden.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly AppDbContext _context;

        public AuthController(UserManager<User> userManager, AppDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromBody] UserRegistrationReqest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var appUser = new User
                {
                    userId = Guid.NewGuid().ToString(),
                    UserName = request.firstName,
                    email = request.email,
                    firstName = request.firstName,
                    lastName = request.lastName,
                    Email = request.email,
                    password = request.password,
                    phone = request.phone

                };

                var createdUser = await _userManager.CreateAsync(appUser, request.password);
                if (createdUser.Succeeded)
                {
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
                        return StatusCode(500, roleResult.Errors);
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
    }
}
