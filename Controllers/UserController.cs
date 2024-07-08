using Iden.DTOs;
using Iden.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Iden.Controllers
{
    [Authorize]
    [Route("api/")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        public UserController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet("users/:id")]
        public async Task<IActionResult> GetUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound(new
                {
                    status = "Not Found",
                    message = "User not found",
                    statusCode = 404
                });
            }

            var response = new UserResponse
            {
                userId = user.userId,
                firstName = user.firstName,
                lastName = user.lastName,
                email = user.Email,
                phone = user.phone
            };

            return Ok(new
            {
                status = "success",
                message = "User retrieved successfully",
                data = response
            });
        }
    }
}
