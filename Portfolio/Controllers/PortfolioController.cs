using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Portfolio.Context;

namespace Portfolio.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class PortfolioController : ControllerBase
	{
		private readonly AppDbContext _context;
		public PortfolioController(AppDbContext appDbContext)
		{
			_context = appDbContext;
		}
		[HttpGet("Getbasicintro")]
		public async Task<IActionResult> getintro()
		{
			try
			{
				var result = await _context.AboutMes
									.Select(a => new
									{
										a.Id,
										a.Description,
										a.Experience,
										a.Imageurl
									}).FirstOrDefaultAsync();

				return Ok(result);

			}
			catch (Exception ex)
			{
				return BadRequest(new { message = ex.Message });
			}
		}
		[HttpGet("getskillsp")]
		public async Task<IActionResult> get_skills()
		{
			try
			{
				var result = await _context.SkillMsts.ToListAsync();
				return Ok(result);
			}
			catch (Exception)
			{
				return BadRequest(new { message = "something went wrong" });
			}
		}
		[HttpGet("getexperience")]
		public async Task<IActionResult> get_experience()
		{
			try
			{
				var result = await _context.ExperienceMsts.ToListAsync();
				return Ok(result);
			}
			catch (Exception)
			{
				return BadRequest(new { message = "Aomething went wrong" });
			}
		}
		[HttpGet("geteducation")]
		public async Task<IActionResult> get_education()
		{
			try
			{
				var result = await _context.EducationMsts.ToListAsync();
				return Ok(result);
			}
			catch (Exception)
			{
				return BadRequest(new { message = "Something went wrong" });
			}
		}
		[HttpGet("Header")]
		public async Task<IActionResult> GetHeader()
		{
			try
			{
				var information = await _context.HeaderInformations.ToListAsync();
				return Ok(information);
			}
			catch (Exception)
			{
				return BadRequest(new { message = "Something went Wrong" });
			}
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
		[HttpGet("GetProjectList")]
		public async Task<IActionResult> Get_Project_list()
		{
			try
			{
				var result = await _context.Projects.ToListAsync();
				return Ok(result);
			}
			catch (Exception)
			{
				return BadRequest(new { message = "Something Went Wrong" });
			}
		}
		[HttpGet("GetSkills")]
		public async Task<IActionResult> Get_skills()
		{
			try
			{
				var result = await _context.MySkills.ToListAsync();
				return Ok(result);
			}
			catch (Exception)
			{
				return BadRequest(new { message = "Something went wrong" });
			}
		}
	}
}
