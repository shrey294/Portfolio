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
		public async Task<IActionResult> UpdateIntro(int id, [FromForm] AboutMeUpdateDto dto)
		{
			try
			{
				var existingRecord = await _context.AboutMes.FindAsync(id);
				if (existingRecord == null)
				{
					return NotFound(new { message = "Record Not Found" });
				}

				// Update text fields
				existingRecord.Description = dto.Description;
				existingRecord.Experience = dto.Experience;

				if (dto.ImageFile != null && dto.ImageFile.Length > 0)
				{
					// Delete old image
					if (!string.IsNullOrEmpty(existingRecord.Imageurl))
					{
						var uri = new Uri(existingRecord.Imageurl);
						var publicId = System.IO.Path.GetFileNameWithoutExtension(uri.LocalPath);
						var deletionParams = new DeletionParams("portfolio/" + publicId);
						await _cloudinary.DestroyAsync(deletionParams);
					}

					// Upload new image
					var uploadParams = new ImageUploadParams
					{
						File = new FileDescription(dto.ImageFile.FileName, dto.ImageFile.OpenReadStream()),
						Folder = "portfolio"
					};

					var uploadResult = await _cloudinary.UploadAsync(uploadParams);
					existingRecord.Imageurl = uploadResult.SecureUrl.ToString();
				}

				await _context.SaveChangesAsync();
				return Ok(new { message = "Record Updated Successfully" });
			}
			catch (Exception ex)
			{
				return BadRequest(new { message = "Something Went Wrong", error = ex.Message });
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
		public async Task<IActionResult> InsertExperience([FromBody] ExperienceMst experienceMst)
		{
			using var transaction = await _context.Database.BeginTransactionAsync();
			try
			{
				await _context.ExperienceMsts.AddAsync(experienceMst);
				await _context.SaveChangesAsync();

				string newExperienceId = experienceMst.Id.ToString();
				var aboutme = await _context.AboutMes.FirstOrDefaultAsync();

				if(aboutme == null)
				{
					return NotFound(new { message = "About_me record not found" });
				}
				List<string> existingExperienceIds = (aboutme.Experienceids ?? "").Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();

				existingExperienceIds.Add(newExperienceId);
				aboutme.Experienceids = string.Join(",", existingExperienceIds.Distinct());
				_context.AboutMes.Update(aboutme);
				await _context.SaveChangesAsync();
				await transaction.CommitAsync();
				return Ok(new { message = "Experience and reference saved successfully" });

			}
			catch (Exception) 
			{
				await transaction.RollbackAsync();
				return BadRequest(new { message = "Something went wrong" });
			}
		}
		[HttpPut("updateexperience")]
		public async Task<IActionResult> UpdateExperience(ExperienceMst updatedExperience)
		{
			using var transaction = await _context.Database.BeginTransactionAsync();
			try
			{
				var existingExperience = await _context.ExperienceMsts.FindAsync(updatedExperience.Id);
				if (existingExperience == null)
				{
					return NotFound(new { message = $"Experience not found" });
				}

				existingExperience.Title = updatedExperience.Title;
				existingExperience.Description = updatedExperience.Description;
				existingExperience.CompanyName = updatedExperience.CompanyName;
				existingExperience.Duration = updatedExperience.Duration;

				await _context.SaveChangesAsync();
				await transaction.CommitAsync();

				return Ok(new { message = "Experience updated successfully" });
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

		//[HttpPost("inserteducation")]
		//public async Task<IActionResult> insert_education(List<EducationMst> educationMsts)
		//{
		//	using var transaction = await _context.Database.BeginTransactionAsync();
		//	try
		//	{
		//		await _context.EducationMsts.AddRangeAsync(educationMsts);
		//		await _context.SaveChangesAsync();

		//		List<string> neweducationIds = educationMsts.Select(x => x.Id.ToString()).ToList();

		//		var aboutMe = await _context.AboutMes.FirstOrDefaultAsync();

		//		if (aboutMe == null)
		//		{
		//			return NotFound(new { message = "About_me record not found" });
		//		}

		//		List<string> existingeducationIds = (aboutMe.Educationids ?? "")
		//										.Split(',', StringSplitOptions.RemoveEmptyEntries)
		//										.ToList();

		//		existingeducationIds.AddRange(neweducationIds);

		//		aboutMe.Educationids = string.Join(",", existingeducationIds.Distinct());

		//		_context.AboutMes.Update(aboutMe);
		//		await _context.SaveChangesAsync();

		//		await transaction.CommitAsync();

		//		return Ok(new { message = "Education and reference saved successfully" });
		//	}
		//	catch (Exception)
		//	{
		//		await transaction.RollbackAsync();
		//		return BadRequest(new { message = "Something went wrong" });
		//	}
		//}

		[HttpPost("inserteducation")]
		public async Task<IActionResult> InsertEducation([FromBody] EducationMst educationMst)
		{
			using var transaction = await _context.Database.BeginTransactionAsync();
			try
			{
				// Add the single EducationMst object
				await _context.EducationMsts.AddAsync(educationMst);
				await _context.SaveChangesAsync();

				string newEducationId = educationMst.Id.ToString();


				// Fetch AboutMe record
				var aboutMe = await _context.AboutMes.FirstOrDefaultAsync();

				if (aboutMe == null)
				{
					return NotFound(new { message = "About_me record not found" });
				}

				// Get existing IDs and add the new one
				List<string> existingEducationIds = (aboutMe.Educationids ?? "")
					.Split(',', StringSplitOptions.RemoveEmptyEntries)
					.ToList();

				existingEducationIds.Add(newEducationId);

				aboutMe.Educationids = string.Join(",", existingEducationIds.Distinct());

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


		//[HttpPut("updateeducation")]
		//public async Task<IActionResult> Update_education(List<EducationMst> updatededucations)
		//{
		//	using var transaction = await _context.Database.BeginTransactionAsync();

		//	try
		//	{
		//		foreach (var updatededucation in updatededucations)
		//		{
		//			var existingeducation = await _context.EducationMsts.FindAsync(updatededucation.Id);
		//			if (existingeducation == null)
		//			{
		//				return NotFound(new { message = $"Skill with ID {updatededucation.Id} not found" });
		//			}

		//			existingeducation.Qualification = updatededucation.Qualification;
		//			existingeducation.CollegeName = updatededucation.CollegeName;
		//			existingeducation.Description = updatededucation.Description;
		//		}

		//		await _context.SaveChangesAsync();
		//		await transaction.CommitAsync();

		//		return Ok(new { message = "Skills updated successfully" });
		//	}
		//	catch (Exception)
		//	{
		//		await transaction.RollbackAsync();
		//		return BadRequest(new { message = "Something went wrong" });
		//	}
		//}
		[HttpPut("updateeducation")]
		public async Task<IActionResult> UpdateEducation([FromBody] EducationMst updatedEducation)
		{
			using var transaction = await _context.Database.BeginTransactionAsync();

			try
			{
				var existingEducation = await _context.EducationMsts.FindAsync(updatedEducation.Id);
				if (existingEducation == null)
				{
					return NotFound(new { message = $"Education with ID {updatedEducation.Id} not found" });
				}

				// Update properties
				existingEducation.Qualification = updatedEducation.Qualification;
				existingEducation.CollegeName = updatedEducation.CollegeName;
				existingEducation.Description = updatedEducation.Description;
				existingEducation.Duration = updatedEducation.Duration;

				await _context.SaveChangesAsync();
				await transaction.CommitAsync();

				return Ok(new { message = "Education updated successfully" });
			}
			catch (Exception)
			{
				await transaction.RollbackAsync();
				return BadRequest(new { message = "Something went wrong" });
			}
		}



	}
}