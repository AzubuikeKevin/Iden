using Iden.AppDBContext;
using Iden.DTOs;
using Iden.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Iden.Controllers
{
    [Authorize]
    [Route("api/")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly AppDbContext _context;
        public UserController(UserManager<User> userManager, AppDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [HttpGet("users/:id")]
        public async Task<IActionResult> GetUser(string id)
        {
            try
            {
                // Find the requesting user by userId
                var requestingUser = await _context.Users.FirstOrDefaultAsync(u => u.userId == id);

                if (requestingUser == null)
                {
                    return NotFound(new
                    {
                        status = "Not Found",
                        message = "Requesting user not found",
                        statusCode = 404
                    });
                }

                // Find the user by userId
                var user = await _context.Users.FirstOrDefaultAsync(u => u.userId == id);

                if (user == null)
                {
                    return NotFound(new
                    {
                        status = "Not Found",
                        message = "User not found",
                        statusCode = 404
                    });
                }

                // Find the user's organisation
                var userOrganisation = await _context.UserOrganisation
                    .Include(uo => uo.Organisation) // Ensure Organisation is included
                    .FirstOrDefaultAsync(uo => uo.userId == user.Id);

                if (userOrganisation == null || userOrganisation.Organisation == null)
                {
                    return NotFound(new
                    {
                        status = "Not Found",
                        message = "User's organisation not found",
                        statusCode = 404
                    });
                }

                // Find all users belonging to the same organisation (including the main user)
                var usersInSameOrganisation = await _context.UserOrganisation
                    .Where(uo => uo.orgId == userOrganisation.orgId)
                    .Select(uo => uo.User)
                    .ToListAsync();

                // Return the user data along with their organisation and other users in the same organisation
                var response = new
                {
                    status = "success",
                    message = "User retrieved successfully",
                    data = usersInSameOrganisation.Select(u => new
                    {
                        userId = u.Id,
                        firstName = u.firstName,
                        lastName = u.lastName,
                        email = u.Email,
                        phone = u.phone
                    }).ToList()
                };

                return Ok(response);
            }
            catch (Exception e)
            {
                return StatusCode(500, new
                {
                    status = "Internal Server Error",
                    message = e.Message,
                    statusCode = 500
                });
            }
        }
    }
}