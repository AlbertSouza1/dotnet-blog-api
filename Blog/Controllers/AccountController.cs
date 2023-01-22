using System.Linq;
using System.Threading.Tasks;
using Blog.Data;
using Blog.Extensions;
using Blog.Models;
using Blog.Services;
using Blog.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SecureIdentity.Password;

namespace Blog.Controllers
{
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly TokenService _tokenService;

        public AccountController(TokenService tokenService)
            => _tokenService = tokenService;

        [HttpPost("v1/users/register")]
        public async Task<IActionResult> CreateAccount([FromBody] RegisterViewModel model, [FromServices] BlogDataContext context)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResultViewModel<string>(ModelState.GetErrors()));

            User user = new()
            {
                Name = model.Name,
                Email = model.Email,
                Slug = model.Email.Replace("@", "-").Replace(".", "-")
            };

            var password = PasswordGenerator.Generate(length: 25);
            user.PasswordHash = PasswordHasher.Hash(password);

            try
            {
                await context.Users.AddAsync(user);
                await context.SaveChangesAsync();

                return CreatedAtAction(actionName: nameof(GetUserName),
                                           routeValues: new { id = user.Id },
                                           value: new ResultViewModel<dynamic>(new
                                           {
                                               user = user.Email,
                                               password
                                           }));
            }
            catch (DbUpdateException)
            {
                return StatusCode(400, new ResultViewModel<string>(error: "This e-mail address is already used."));
            }
            catch
            {
                return StatusCode(500, new ResultViewModel<string>(error: "Internal error."));
            }
        }

        [HttpPost("v1/login")]
        public IActionResult Login()
        {
            var token = _tokenService.GenerateToken(null);

            return Ok(token);
        }

        [HttpGet("v1/users/{id}")]
        public IActionResult GetUserName([FromServices] BlogDataContext context, [FromRoute] int id)
        {
            try
            {
                var user = context.Users.FirstOrDefault(x => x.Id == id);
                if (user is null)
                    return BadRequest(new ResultViewModel<User>(error: "User not found."));

                return Ok(new ResultViewModel<string>(data: user.Name));
            }
            catch
            {
                return StatusCode(500, new ResultViewModel<User>(error: "Internal error."));
            }
        }
    }
}