using Iden.AppDBContext;
using Iden.DTOs;
using Iden.Entities;
using Iden.Helper;
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
                            org.orgId,
                            org.name,
                            org.description
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

        [HttpGet("organisations/:orgId")]
        public async Task<IActionResult> GetOrganisationById(string orgId)
        {
            var organisation = await _context.Organization.FindAsync(orgId);

            if (organisation == null)
            {
                return NotFound(new
                {
                    status = "fail",
                    message = "Organisation not found"
                });
            }

            var response = new
            {
                status = "success",
                message = "Organisation retrieved successfully",
                data = new
                {
                    orgId = organisation.orgId,
                    name = organisation.name,
                    description = organisation.description
                }
            };

            return Ok(response);
        }

        [HttpPost("organisations")]
        public async Task<IActionResult> CreateOrganisation([FromBody] CreateOrganisationRequest request)
        {
            if (!ModelState.IsValid)
            {
                var validationErrors = ValidationHelper.GetValidationErrors(ModelState, typeof(CreateOrganisationRequest));
                return BadRequest(new
                {
                    status = "Bad Request",
                    message = "Client error",
                    errors = validationErrors
                });
            }

            try
            {
                var userId = User.FindFirstValue(ClaimTypes.GivenName);
                var user = await _userManager.FindByNameAsync(userId);

                if (user == null)
                {
                    return Unauthorized(new
                    {
                        status = "Unauthorized",
                        message = "User not found or not logged in",
                        statusCode = 401
                    });
                }

                var organisation = new Organisation
                {
                    orgId = Guid.NewGuid().ToString(),
                    name = request.Name,
                    description = request.Description,
                    UserOrganisations = new List<UserOrganisation>
            {
                new UserOrganisation
                {
                    userId = user.Id
                }
            }
                };

                _context.Organization.Add(organisation);
                await _context.SaveChangesAsync();

                var response = new
                {
                    status = "success",
                    message = "Organisation created successfully",
                    data = new
                    {
                        organisation.orgId,
                        organisation.name,
                        organisation.description
                    }
                };

                return CreatedAtAction(nameof(CreateOrganisation), response);
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

        [HttpPost("organisations/:orgId/users")]
        public async Task<IActionResult> AddUserToOrganisation(string orgId, [FromBody] AddUserToOrganisationRequest request)
        {
            if (!ModelState.IsValid)
            {
                var validationErrors = ValidationHelper.GetValidationErrors(ModelState, typeof(AddUserToOrganisationRequest));
                return BadRequest(new
                {
                    status = "Bad Request",
                    message = "Client error",
                    errors = validationErrors
                });
            }

            try
            {
                var organisation = await _context.Organization.FindAsync(orgId);

                if (organisation == null)
                {
                    return NotFound(new
                    {
                        status = "Not Found",
                        message = "Organisation not found",
                        statusCode = 404
                    });
                }

                var user = await _context.Users.FirstOrDefaultAsync(u => u.userId == request.userId);

                if (user == null)
                {
                    return NotFound(new
                    {
                        status = "Not Found",
                        message = "User not found",
                        statusCode = 404
                    });
                }
                var userAlreadyInOrganisation = await _context.UserOrganisation
                    .AnyAsync(uo => uo.userId == user.Id && uo.orgId == orgId);

                if (userAlreadyInOrganisation)
                {
                    return BadRequest(new
                    {
                        status = "Bad Request",
                        message = "User is already a member of this organisation",
                        statusCode = 400
                    });
                }

                var userOrganisation = new UserOrganisation
                {
                    userId = user.Id,
                    orgId = organisation.orgId
                };

                _context.UserOrganisation.Add(userOrganisation);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    status = "success",
                    message = "User added to organisation successfully",
                });
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
