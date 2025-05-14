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
	public class SkillsController : ControllerBase
	{
		private readonly AppDbContext _context;

		public SkillsController(AppDbContext appDbContext)
		{
			_context = appDbContext;
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
				return BadRequest(new {message="Something went wrong"});
			}
		}
		[HttpPost("Insertskills")]
		public async Task<IActionResult> add_skills(List<MySkill> mySkills)
		{
			try
			{
				await _context.MySkills.AddRangeAsync(mySkills);
				await _context.SaveChangesAsync();
				return Ok(new {message="Skills Inserted Successfully"});
			}
			catch (Exception)
			{
				return BadRequest(new { message = "Something went wrong" });
			}
		}
		[HttpPut("UpdatedSkills/{id}")]
		public async Task<IActionResult> Update_skills(int id, MySkill skill)
		{
			try
			{
				var skilltoupdated = await _context.MySkills.FindAsync(id);
				if (skilltoupdated == null) 
				{
					return BadRequest(new {message="Skill with given Id Not found"});
				}
				skilltoupdated.Title = skill.Title;
				skilltoupdated.Imageurl = skill.Imageurl;
				skilltoupdated.PillText = skill.PillText;
				await _context.SaveChangesAsync();
				return Ok(new {message="Skill updated Successfully"});
			}
			catch (Exception) 
			{
				return BadRequest(new {message="Something Went Wrong"});
			}
		}
		[HttpDelete("deleteskill/{id}")]
		public async Task<IActionResult> Deleteskill(int id)
		{
			try
			{
				if (id == 0)
				{
					return NotFound(new { message = "Skill with given id not found" });
				}

				var skill = await _context.MySkills.FindAsync(id);
				if (skill == null)
				{
					return NotFound(new { message = "Skill with given id not found" });
				}

				_context.MySkills.Remove(skill);
				await _context.SaveChangesAsync();

				return Ok(new { message = "Skill Deleted Successfully" });
			}
			catch (Exception)
			{
				return BadRequest(new { message = "Something went wrong" });
			}
		}

	}
}
