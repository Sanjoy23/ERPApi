using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApp.Data;
using WebApp.Helper;
using WebApp.Models;

namespace WebApp.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class AccountsController : ControllerBase
	{
		private readonly ERPDbContext _context;
		private readonly IConfiguration _configuration;

		public AccountsController(ERPDbContext context, IConfiguration configuration)
		{
			_context = context;
			_configuration = configuration;
		}

		[HttpPost("register")]
		public async Task<IActionResult> Register(string username, string password)
		{
			if (await _context.Users.AnyAsync(u => u.Username == username))
				return BadRequest("Username already exists");

			var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);

			var user = new User
			{
				Username = username,
				PasswordHash = passwordHash
			};

			_context.Users.Add(user);
			await _context.SaveChangesAsync();

			return Ok("User registered successfully");
		}

		[HttpPost("login")]
		public async Task<IActionResult> Login(string username, string password)
		{
			var user = await _context.Users.SingleOrDefaultAsync(u => u.Username == username);

			if (user == null || user.IsLockedOut)
				return Unauthorized("Invalid credentials or account locked");

			if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
			{
				user.FailedLoginAttempts++;

				if (user.FailedLoginAttempts >= 5)
				{
					user.IsLockedOut = true;
					user.LockoutEnd = DateTime.UtcNow.AddMinutes(15); // Lockout for 15 minutes
				}

				await _context.SaveChangesAsync();
				return Unauthorized("Invalid credentials");
			}

			user.FailedLoginAttempts = 0;
			user.IsLockedOut = false;
			user.LockoutEnd = null;

			await _context.SaveChangesAsync();

			var token = GenerateJwtToken(user);
			return Ok(new { Token = token });
		}

		private string GenerateJwtToken(User user)
		{
			var jwtSettings = _configuration.GetSection("JwtSettings").Get<JwtSettings>();
			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var claims = new[]
			{
				new Claim(JwtRegisteredClaimNames.Sub, user.Username),
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
			};

			var token = new JwtSecurityToken(
				issuer: jwtSettings.Issuer,
				audience: jwtSettings.Audience,
				claims: claims,
				expires: DateTime.UtcNow.AddMinutes(jwtSettings.ExpiresInMinutes),
				signingCredentials: creds
			);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}



	}
}
