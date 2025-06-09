using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
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
	public class ProjectsController : ControllerBase
	{
		private readonly AppDbContext _context;
		private readonly Cloudinary _cloudinary;

		public ProjectsController(AppDbContext appDbContext, Cloudinary cloudinary)
		{
			_context = appDbContext;
			_cloudinary = cloudinary;
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
		[HttpPost("AddProject")]
		public async Task<IActionResult> Add_project([FromForm] IFormCollection formdata)
		{
			try
			{
				var imageFile = formdata.Files["imageFile"];
				string? imageurl = null;
				if (imageFile != null && imageFile.Length > 0) 
				{
					var uploadParams = new ImageUploadParams
					{
						File = new FileDescription(imageFile.FileName,imageFile.OpenReadStream()),
						Folder = "Projects"
					};
					var uploadresult = await _cloudinary.UploadAsync(uploadParams);
					imageurl = uploadresult.SecureUrl.ToString();
				}
				var projects = new Project
				{
					Link = formdata["Link"],
					Imageurl = imageurl,
					Title = formdata["Title"],
					ShortDescription = formdata["ShortDescription"],
					StackPill = formdata["StackPill"]
				};

				await _context.Projects.AddAsync(projects);
				await _context.SaveChangesAsync();
				return Ok(new {message="Project inserted Successfully"});
			}
			catch (Exception) 
			{
				return BadRequest(new {message="Something went Wrong" });
			}
		}
		[HttpPut("updateproject/{id}")]
		public async Task<IActionResult> update_project(int id, [FromForm] IFormCollection formdata)
		{
			try
			{
				var existingrecord = await _context.Projects.FindAsync(id);
				if (existingrecord != null) 
				{
					existingrecord.Title = formdata["Title"];
					existingrecord.Link = formdata["Link"];
					existingrecord.ShortDescription = formdata["ShortDescription"];
					existingrecord.StackPill = formdata["StackPill"];

					var imageFile = formdata.Files["imageFile"];
					if (imageFile !=null && imageFile.Length > 0)
					{
						//delete old image
						if(!string.IsNullOrEmpty(existingrecord.Imageurl))
						{
							var uri = new Uri(existingrecord.Imageurl);
							var publicId = System.IO.Path.GetFileNameWithoutExtension(uri.LocalPath);
							var deletionparams = new DeletionParams("Projects/" + publicId);
							await _cloudinary.DestroyAsync(deletionparams);
						}
						// upload new image
						var uploadparams = new ImageUploadParams
						{
							File = new FileDescription(imageFile.FileName, imageFile.OpenReadStream()),
							Folder = "Projects"
						};
						var uploadresult = await _cloudinary.UploadAsync(uploadparams);
						existingrecord.Imageurl = uploadresult.SecureUrl.ToString();
					}
					await _context.SaveChangesAsync();
					return Ok(new { message = "Record updated Successfully" });
				}
				else
				{
					return NotFound(new {message="Record Not Found"});
				}
			}
			catch (Exception) 
			{
				return BadRequest(new {message="Something Went Wrong"});
			}
		}
		[HttpDelete("deleteproject/{id}")]
		public async Task<IActionResult> delete_project(int id)
		{
			try
			{
				var recordtodelte = await _context.Projects.FindAsync(id);
				if (recordtodelte != null)
				{
					_context.Projects.Remove(recordtodelte);
					await _context.SaveChangesAsync();
					return Ok(new {message="Record Deleted Successfully"});
				}
				else
				{
					return NotFound(new { message = "Record Not Found" });
				}
			}
			catch (Exception) 
			{
				return BadRequest(new {message="Something Went Wrong"});
			}
		}
	}
}
