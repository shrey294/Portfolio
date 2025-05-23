using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Portfolio.Context;
using Portfolio.Models;
using Portfolio.Models.DTO;

namespace Portfolio.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	[Authorize]
	public class About_meController : ControllerBase
	{
		private readonly AppDbContext _context;
		private readonly Cloudinary _cloudinary;

		public About_meController(AppDbContext appDbContext, Cloudinary cloudinary)
		{
			_context = appDbContext;
			_cloudinary = cloudinary;
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
			catch (Exception)
			{
				return BadRequest(new { message = "Something went wrong" });
			}
		}

		
		[HttpPost("Addintro")]
		public async Task<IActionResult> Insert_intro([FromForm] IFormCollection formData)
		{
			try
			{
				var imageFile = formData.Files["imageFile"];
				string imageUrl = null;

				if (imageFile != null && imageFile.Length > 0)
				{
					var uploadParams = new ImageUploadParams
					{
						File = new FileDescription(imageFile.FileName, imageFile.OpenReadStream()),
						Folder = "portfolio"
					};

					var uploadResult = await _cloudinary.UploadAsync(uploadParams);
					imageUrl = uploadResult.SecureUrl.ToString();
				}

				var aboutme = new AboutMe
				{
					Description = formData["Description"],
					Experience = double.Parse(formData["Experience"]),
					Imageurl = imageUrl
				};

				await _context.AboutMes.AddAsync(aboutme);
				await _context.SaveChangesAsync();

				return Ok(new { message = "Description added Successfully" });
			}
			catch (Exception ex)
			{
				return BadRequest("Something went wrong: " + ex.Message);
			}
		}

		[HttpPut("updateintro/{id}")]
		public async Task<IActionResult> update_intro(int id, AboutMe aboutMe)
		{
			try
			{
				var descriptiontoupdate = await _context.AboutMes.FindAsync(id);
				if (descriptiontoupdate == null)
				{
					return BadRequest(new { message = "Record Not Found" });
				}
				else
				{
					descriptiontoupdate.Description = aboutMe.Description;
					descriptiontoupdate.Imageurl = aboutMe.Imageurl;
					descriptiontoupdate.Experience = aboutMe.Experience;
					await _context.SaveChangesAsync();
					return Ok(new { message = "Record Update Successfully" });
				}
			}
			catch (Exception)
			{
				return BadRequest(new { message = "Something Went Wrong" });
			}
		}
		[HttpGet("getskills")]
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
		[HttpPost("addskills")]
		public async Task<IActionResult> InsertSkills(List<SkillMst> skillMsts)
		{
			using var transaction = await _context.Database.BeginTransactionAsync();

			try
			{
				// Step 1: Insert all skills
				await _context.SkillMsts.AddRangeAsync(skillMsts);
				await _context.SaveChangesAsync();


				// Step 2: Get new skill IDs
				List<string> newSkillIds = skillMsts.Select(s => s.Id.ToString()).ToList();

				// Step 3: Get the About_me row to update
				var aboutMe = await _context.AboutMes.FirstOrDefaultAsync();

				if (aboutMe == null)
				{
					return NotFound(new { message = "About_me record not found" });
				}

				// Step 4: Append the new skill IDs to the existing list
				List<string> existingSkillIds = (aboutMe.Skillids ?? "")
												.Split(',', StringSplitOptions.RemoveEmptyEntries)
												.ToList();

				existingSkillIds.AddRange(newSkillIds);

				aboutMe.Skillids = string.Join(",", existingSkillIds.Distinct()); // Avoid duplicates

				_context.AboutMes.Update(aboutMe);
				await _context.SaveChangesAsync();

				await transaction.CommitAsync();

				return Ok(new { message = "Skills and reference saved successfully" });
			}
			catch (Exception)
			{
				await transaction.RollbackAsync();
				return BadRequest(new { message = "Something went wrong"});
			}
		}

		[HttpPut("updateskills")]
		public async Task<IActionResult> UpdateSkills(List<SkillMst> updatedSkills)
		{
			using var transaction = await _context.Database.BeginTransactionAsync();

			try
			{
				foreach (var updatedSkill in updatedSkills)
				{
					var existingSkill = await _context.SkillMsts.FindAsync(updatedSkill.Id);
					if (existingSkill == null)
					{
						return NotFound(new { message = $"Skill with ID {updatedSkill.Id} not found" });
					}

					existingSkill.SkillName = updatedSkill.SkillName;
					existingSkill.SkillPercantage = updatedSkill.SkillPercantage;
				}

				await _context.SaveChangesAsync();
				await transaction.CommitAsync();

				return Ok(new { message = "Skills updated successfully" });
			}
			catch (Exception)
			{
				await transaction.RollbackAsync();
				return BadRequest(new { message = "Something went wrong"});
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

		[HttpPost("insertexperience")]
		public async Task<IActionResult> insert_experience(List<ExperienceMst> experienceMsts)
		{
			using var transaction = await _context.Database.BeginTransactionAsync();
			try
			{
				await _context.ExperienceMsts.AddRangeAsync(experienceMsts);
				await _context.SaveChangesAsync();

				List<string> newexperienceIds = experienceMsts.Select(x => x.Id.ToString()).ToList();

				var aboutMe = await _context.AboutMes.FirstOrDefaultAsync();

				if (aboutMe == null)
				{
					return NotFound(new { message = "About_me record not found" });
				}

				List<string> existingexperienceIds = (aboutMe.Experienceids ?? "")
												.Split(',', StringSplitOptions.RemoveEmptyEntries)
												.ToList();

				existingexperienceIds.AddRange(newexperienceIds);

				aboutMe.Experienceids = string.Join(",", existingexperienceIds.Distinct());

				_context.AboutMes.Update(aboutMe);
				await _context.SaveChangesAsync();

				await transaction.CommitAsync();

				return Ok(new { message = "Experiences and reference saved successfully" });
			}
			catch (Exception)
			{
				await transaction.RollbackAsync();
				return BadRequest(new { message = "Something went wrong" });
			}
		}
		[HttpPut("updatedexperience")]
		public async Task<IActionResult> update_experience(List<ExperienceMst> experienceMstsupdated)
		{
			using var transaction = await _context.Database.BeginTransactionAsync();
			try
			{
				foreach (var updatedexperience in experienceMstsupdated) 
				{
					var existingexperience = await _context.ExperienceMsts.FindAsync(updatedexperience.Id);
					if (existingexperience == null) 
					{
						return NotFound(new { message = $"Experience with ID {updatedexperience.Id} not found" });
					}
					existingexperience.Title = updatedexperience.Title;
					existingexperience.Description = updatedexperience.Description;
					existingexperience.CompanyName = updatedexperience.CompanyName;
				}
				await _context.SaveChangesAsync();
				await transaction.CommitAsync();
				return Ok(new {message="experience updated successfully"});
			}
			catch (Exception) 
			{
				await transaction.RollbackAsync();
				return BadRequest(new { message = "Something went wrong" });
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

		[HttpPost("inserteducation")]
		public async Task<IActionResult> insert_education(List<EducationMst> educationMsts)
		{
			using var transaction = await _context.Database.BeginTransactionAsync();
			try
			{
				await _context.EducationMsts.AddRangeAsync(educationMsts);
				await _context.SaveChangesAsync();

				List<string> neweducationIds = educationMsts.Select(x => x.Id.ToString()).ToList();

				var aboutMe = await _context.AboutMes.FirstOrDefaultAsync();

				if (aboutMe == null)
				{
					return NotFound(new { message = "About_me record not found" });
				}

				List<string> existingeducationIds = (aboutMe.Educationids ?? "")
												.Split(',', StringSplitOptions.RemoveEmptyEntries)
												.ToList();

				existingeducationIds.AddRange(neweducationIds);

				aboutMe.Educationids = string.Join(",", existingeducationIds.Distinct());

				_context.AboutMes.Update(aboutMe);
				await _context.SaveChangesAsync();

				await transaction.CommitAsync();

				return Ok(new { message = "Education and reference saved successfully" });
			}
			catch (Exception)
			{
				await transaction.RollbackAsync();
				return BadRequest(new { message = "Something went wrong" });
			}
		}

		[HttpPut("updateeducation")]
		public async Task<IActionResult> Update_education(List<EducationMst> updatededucations)
		{
			using var transaction = await _context.Database.BeginTransactionAsync();

			try
			{
				foreach (var updatededucation in updatededucations)
				{
					var existingeducation = await _context.EducationMsts.FindAsync(updatededucation.Id);
					if (existingeducation == null)
					{
						return NotFound(new { message = $"Skill with ID {updatededucation.Id} not found" });
					}

					existingeducation.Qualification = updatededucation.Qualification;
					existingeducation.CollegeName = updatededucation.CollegeName;
					existingeducation.Description = updatededucation.Description;
				}

				await _context.SaveChangesAsync();
				await transaction.CommitAsync();

				return Ok(new { message = "Skills updated successfully" });
			}
			catch (Exception)
			{
				await transaction.RollbackAsync();
				return BadRequest(new { message = "Something went wrong" });
			}
		}
		
	}
}
