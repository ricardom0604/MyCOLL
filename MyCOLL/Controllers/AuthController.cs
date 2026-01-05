using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MyCOLL.Data.Data;
using MyCOLL.Shared.Constants;
using MyCOLL.Shared.Models.Dto;

namespace MyCOLL.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
	private readonly UserManager<ApplicationUser> _userManager;
	private readonly IConfiguration _configuration;

	public AuthController(UserManager<ApplicationUser> userManager, IConfiguration configuration)
	{
		_userManager = userManager;
		_configuration = configuration;
	}

	[HttpPost("register")]
	public async Task<IActionResult> Register([FromBody] RegisterDto dto)
	{
		if (dto.Role != "Cliente" && dto.Role != "Fornecedor")
			return BadRequest(new { Message = "Invalid role specified" });

		var user = new ApplicationUser
		{
			UserName = dto.Email,
			Email = dto.Email,
			FullName = dto.FullName,
			Nif = dto.Nif,
			Address = dto.Address,
			StateAccount = StateAccount.Pending
		};
		var result = await _userManager.CreateAsync(user, dto.Password);

		if (!result.Succeeded)
			return BadRequest(result.Errors);

		var roleToAssign = UserRoles.Client;
		if (dto.Role == UserRoles.Supplier)
			roleToAssign = UserRoles.Supplier;

		await _userManager.AddToRoleAsync(user, roleToAssign);
		return Ok(new AuthResponseDto
		{
			IsSuccess = true,
			Message = "User registered successfully"
		});
	}

	[HttpPost("login")]
	public async Task<IActionResult> Login([FromBody] LoginDto dto)
	{
		var user = await _userManager.FindByEmailAsync(dto.Email);
		if (user != null || await _userManager.CheckPasswordAsync(user, dto.Password))
		{
			var authClaims = new List<Claim>
			{
				new Claim(ClaimTypes.Name, user.UserName),
				new Claim(ClaimTypes.NameIdentifier, user.Id),
				new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
			};

			var userRoles = await _userManager.GetRolesAsync(user);
			foreach (var role in userRoles)
			{
				authClaims.Add(new Claim(ClaimTypes.Role, role));
			}

			var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));

			var token = new JwtSecurityToken(
				issuer: _configuration["Jwt:Issuer"],
				audience: _configuration["Jwt:Issuer"],
				expires: DateTime.Now.AddHours(3),
				claims: authClaims,
				signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
			);

			return Ok(new
			{
				token = new JwtSecurityTokenHandler().WriteToken(token),
				expiration = token.ValidTo
			});
		}
		return Unauthorized();
	}
}
