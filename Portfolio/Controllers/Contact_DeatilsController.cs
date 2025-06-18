using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Portfolio.Context;
using Portfolio.Models;

namespace Portfolio.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class Contact_DeatilsController : ControllerBase
	{
		private readonly AppDbContext _context;

		public Contact_DeatilsController(AppDbContext appDbContext)
		{
			_context = appDbContext;
		}
		[HttpGet("GetContactDetails")]
		public async Task<IActionResult> Get_Conatct_details()
		{
			try
			{
				var result = await _context.ContactDetails.ToListAsync();
				return Ok(result);
			}
			catch (Exception)
			{
				return BadRequest(new { Message = "Something Went Wrong" });
			}
		}
		[HttpPost("AddContact")]
		public async Task<IActionResult> Add_Contact_details([FromBody] ContactDetail contactDetail)
		{
			try
			{
				await _context.ContactDetails.AddAsync(contactDetail);
				await _context.SaveChangesAsync();
				return Ok(new { Message = "Contact Details Added Successfully" });
			}
			catch (Exception)
			{
				return BadRequest(new { Message = "Something Went Wrong" });
			}
		}
		[HttpPut("UpdateContact")]
		public async Task<IActionResult> update_contact_details(ContactDetail contactDetail)
		{
			try
			{
				var recordtouopdate = await _context.ContactDetails.FirstOrDefaultAsync();
				if (recordtouopdate != null) 
				{
					recordtouopdate.Location = contactDetail.Location;
					recordtouopdate.Email = contactDetail.Email;
					recordtouopdate.Phone = contactDetail.Phone;
					recordtouopdate.GithubUrl = contactDetail.GithubUrl;
					recordtouopdate.IntsaUrl = contactDetail.IntsaUrl;
					recordtouopdate.LinkedInUrl = contactDetail.LinkedInUrl;
					await _context.SaveChangesAsync();
					return Ok(new {Message="Record Upddated Successfully"});
				}
				else
				{
					return NotFound(new {Message="Record Not Found"});
				}
			}
			catch (Exception) 
			{
				return BadRequest(new { Message = "Something Went Wrong" });
			}
		}
	}
}