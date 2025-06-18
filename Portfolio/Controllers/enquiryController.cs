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
	public class enquiryController : ControllerBase
	{
		private readonly AppDbContext _context;
		public enquiryController(AppDbContext appDbContext)
		{
			_context = appDbContext;
		}
		[HttpPost("postenquiry")]
		public async Task<IActionResult> enquiry(Enquiry enquiry)
		{
			try
			{
				await _context.Enquiries.AddAsync(enquiry);
				await _context.SaveChangesAsync();
				return Ok(new { Message = "Data Saved Successfully" });
			}
			catch (Exception)
			{
				return BadRequest(new { Message = "Something Went Wrong While Saving Data" });
			}
		}
		[Authorize]
		[HttpGet("getenquiry")]
		public async Task<IActionResult> Get()
		{
			try
			{
				var result = await _context.Enquiries.ToListAsync();
				return Ok(result);
			}
			catch (Exception)
			{
				return BadRequest(new {Message="Something Wen Wrong"});
			}
		}
	}
}
