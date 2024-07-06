using Iden.AppDBContext;
using Iden.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Iden.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/")]
    public class OrganisationController : ControllerBase
    {
        public readonly UserManager<User> _userManager;
        private readonly AppDbContext _context;

        public OrganisationController(UserManager<User> userManager, AppDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [HttpGet("organisations")]
        public async Task<IActionResult> Organisations()
        {
            try
            {
                var userName = User.FindFirstValue(ClaimTypes.GivenName);
                var user = await _userManager.FindByNameAsync(userName);

                if (string.IsNullOrEmpty(userName))
                {
                    return Unauthorized(new
                    {
                        status = "Unauthorized",
                        message = "User not authenticated properly",
                        statusCode = 401
                    });
                }

                if (user == null)
                {
                    return Unauthorized(new
                    {
                        status = "Unauthorized",
                        message = "User not found or not logged in",
                        statusCode = 401
                    });
                }

                var organisations = await _context.UserOrganisation
                    .Where(uo => uo.userId == user.Id)
                    .Select(uo => uo.Organisation)
                    .ToListAsync();

                var response = new
                {
                    status = "success",
                    message = "Organisations retrieved successfully",
                    data = new
                    {
                        organisations = organisations.Select(org => new
                        {
                            orgId = org.orgId,
                            name = org.name,
                            description = org.description
                        })
                    }
                };

                return Ok(response);
            }
            catch (Exception e)
            {

                return StatusCode(500, new
                {
                    status = "Validation Error",
                    message = e.Message,
                    statusCode = 500
                });
            }
        }
    }
}
