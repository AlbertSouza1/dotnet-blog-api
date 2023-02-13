using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Blog.Data;
using Blog.DTOs;
using Blog.Extensions;
using Blog.Models;
using Blog.Services;
using Blog.ViewModels;
using Blog.ViewModels.Accounts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SecureIdentity.Password;
using Blog.Settings;

namespace Blog.Controllers
{
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly TokenService _tokenService;

        public AccountController(TokenService tokenService)
            => _tokenService = tokenService;

        [HttpPost("v1/users/register")]
        public async Task<IActionResult> CreateAccount([FromBody] RegisterViewModel model,
                                                       [FromServices] BlogDataContext context,
                                                       [FromServices] EmailService emailService,
                                                       [FromServices] IConfiguration configuration)
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

                var emailData = new EmailDataDTO(toEmail: user.Email,
                                                 toName: user.Name,
                                                 subject: "Welcome to our blog!",
                                                 body: $"Your password is <strong>{password}.</strong>");

                var smtp = new SmtpConfiguration();
                configuration.GetSection("Smtp").Bind(smtp);

                emailService.Send(emailData, smtp);

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
        public async Task<IActionResult> Login([FromBody] LoginViewModel model,
                                               [FromServices] BlogDataContext context,
                                               [FromServices] TokenService tokenService,
                                               [FromServices] IConfiguration configuration)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ResultViewModel<List<string>>(ModelState.GetErrors()));

            try
            {
                var user = await context.Users
                        .AsNoTracking()
                        .Include(x => x.Roles)
                        .FirstOrDefaultAsync(x => x.Email == model.Email);

                if (user is null)
                    return StatusCode(401, new ResultViewModel<string>(error: "Email or password invalid."));

                if (!PasswordHasher.Verify(user.PasswordHash, model.Password))
                    return StatusCode(401, new ResultViewModel<string>(error: "Email or password invalid."));

                var jwtKey = configuration.GetValue<string>("JwtKey");
                var token = _tokenService.GenerateToken(user, jwtKey);

                return Ok(new ResultViewModel<string>(data: token));
            }
            catch
            {
                return StatusCode(500, new ResultViewModel<string>(error: "Internal error."));
            }
        }

        [Authorize]
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

        [Authorize]
        [HttpPost("v1/users/upload-image")]
        public async Task<IActionResult> UploadImage([FromBody] UploadImageViewModel viewModel, [FromServices] BlogDataContext context)
        {
            var fileName = $"{Guid.NewGuid().ToString()}.jpg";
            var data = new Regex(@"^data:imageV[a-z]+;base64,")
            .Replace(viewModel.Base64Image, "");

            var bytes = Convert.FromBase64String(data);

            try
            {
                await System.IO.File.WriteAllBytesAsync($"wwwroot/images/{fileName}", bytes);

                var user = await context.Users.FirstOrDefaultAsync(x => x.Email.Equals(User.Identity.Name));

                if (user is null)
                    return NotFound(new ResultViewModel<User>(error: "User not found."));

                user.Image = $"https://localhost:5001/images/{fileName}";

                context.Users.Update(user);
                await context.SaveChangesAsync();

                return Ok(new ResultViewModel<string>(data: "User image updated successfully."));
            }
            catch
            {
                return StatusCode(500, new ResultViewModel<string>(data: "Internal error."));
            }
        }
    }
}