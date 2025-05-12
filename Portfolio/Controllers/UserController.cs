using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Portfolio.Context;
using Portfolio.Helpers;
using Portfolio.Models;
using Portfolio.Models.DTO;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Portfolio.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class UserController : ControllerBase
	{
		private readonly AppDbContext _context;

		public UserController(AppDbContext appDbContext)
		{
			_context = appDbContext;
		}

		[HttpPost("authenticate")]
		public async Task<IActionResult> Authenticate([FromBody] UserMst user)
		{
			try
			{
				if (user == null)
				{
					return BadRequest();
				}
				else
				{
					var User = await _context.UserMsts.FirstOrDefaultAsync(x => x.UserName == user.UserName);
					if (User == null)
					{
						return NotFound(new { Message = "User Not Found" });
					}
					if (!PasswordEncrypt.verifyPassword(user.Password, User.Password))
					{
						return BadRequest(new { Message = "Password is incorrect" });
					}
					User.Token = CreateJwttoken(User);
					var newAccessToken = User.Token;
					var newrefreshtoken = CreateRereshToken();
					User.RefreshToken = newrefreshtoken;
					User.Expirytime = DateTime.Now.AddDays(5);
					await _context.SaveChangesAsync();
					return Ok(new TokenApiDto()
					{
						AccessToken = newAccessToken,
						RefreshToken = newrefreshtoken
					});

				}
			}
			catch (Exception ex)
			{
				return BadRequest(ex.Message);
			}
		}
		private string CreateJwttoken(UserMst user)
		{
			var jwtTokenHandler = new JwtSecurityTokenHandler();
			//var key = Encoding.ASCII.GetBytes("APIKEYJWTSECREteKey");
			var key = Encoding.ASCII.GetBytes("SuperSecretKey12345678901234567890"); 
			var identity = new ClaimsIdentity(new Claim[]
			{
				new Claim(ClaimTypes.Name,$"{user.UserName}")
			});
			var Credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = identity,
				Expires = DateTime.Now.AddHours(1),
				SigningCredentials = Credentials,
			};
			var token = jwtTokenHandler.CreateToken(tokenDescriptor);
			return jwtTokenHandler.WriteToken(token);
		}
		private string CreateRereshToken()
		{
			var tokenbytes = RandomNumberGenerator.GetBytes(64);
			var refreshToken = Convert.ToBase64String(tokenbytes);

			var tokeninuser = _context.UserMsts.Any(a => a.RefreshToken == refreshToken);
			if (tokeninuser)
			{
				return CreateRereshToken();
			}
			return refreshToken;
		}
		[HttpPost("Refresh")]
		public async Task<IActionResult> Refresh(TokenApiDto token)
		{
			if (token == null)
			{
				return BadRequest("Invalid Client");
			}
			string accesstoken = token.AccessToken;
			string refreshtoken = token.RefreshToken;

			var principal = GetPrincipalFromExpiredToken(accesstoken);
			var username = principal.Identity.Name;
			var user = await _context.UserMsts.FirstOrDefaultAsync(x => x.UserName == username);
			if (user == null || user.RefreshToken != refreshtoken || user.Expirytime <= DateTime.Now)
			{
				return BadRequest("Invalid Request");
			}
			var newAccessToken = CreateJwttoken(user);
			var RefreshToken = CreateRereshToken();
			user.RefreshToken = RefreshToken;
			await _context.SaveChangesAsync();
			return Ok(new TokenApiDto
			{
				AccessToken = newAccessToken,
				RefreshToken = RefreshToken,
			});
		}

		private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
		{
			var key = Encoding.ASCII.GetBytes("SuperSecretKey12345678901234567890");
			var tokenValidationParameters = new TokenValidationParameters
			{
				ValidateAudience = false,
				ValidateIssuer = false,
				ValidateIssuerSigningKey = true,
				IssuerSigningKey = new SymmetricSecurityKey(key),
				ValidateLifetime = false
			};
			var TokenHandler = new JwtSecurityTokenHandler();
			SecurityToken securityToken;
			var principal = TokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
			var jwtSecurityToken = securityToken as JwtSecurityToken;
			if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
			{
				throw new SecurityTokenException("This is Invalid Token");
			}
			return principal;

		}
	}
}
