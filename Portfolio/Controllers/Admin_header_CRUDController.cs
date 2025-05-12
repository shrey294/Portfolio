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
	public class Admin_header_CRUDController : ControllerBase
	{
		private readonly AppDbContext _context;

		public Admin_header_CRUDController(AppDbContext appDbContext)
		{
			_context = appDbContext;
		}
		[HttpPost("Insert")]
		public async Task<IActionResult> Header_Insert(HeaderInformation headerInformation)
		{
			try
			{
				await _context.HeaderInformations.AddAsync(headerInformation);
				await _context.SaveChangesAsync();
				return Ok(new {message="Header Information saved successfully"});
			}
			catch(Exception)
			{
				return BadRequest(new {message= "Something went Wrong" });
			}
		}
		[HttpGet("Header")]
		public async Task<IActionResult> GetHeader()
		{
			try
			{
				var information = await _context.HeaderInformations.Where(x=>x.IsDelete=="N").ToListAsync();
				return Ok(information);
			}
			catch (Exception) 
			{
				return BadRequest(new {message= "Something went Wrong" });
			}
		}
		[HttpPut("update/{id}")]
		public async Task<IActionResult> UpdateHeader(int id,HeaderInformation headerInformation)
		{
			try
			{
				if (id == 0)
				{
					return BadRequest(new {message="Id is null here"});
				}
				else
				{
					var informationbyid = await _context.HeaderInformations.FirstOrDefaultAsync(x => x.Id == id);

					if (informationbyid == null)
					{
						return NotFound();
					}
					else
					{
						informationbyid.Name = headerInformation.Name;
						informationbyid.ShortDescription = headerInformation.ShortDescription;
						informationbyid.Designation = headerInformation.Designation;
						informationbyid.Initials = headerInformation.Initials;
						informationbyid.Icons = headerInformation.Icons;
						await _context.SaveChangesAsync();
						return Ok(new {message= "information Updated Successfully" });
					}
				}
			}
			catch (Exception) 
			{
				return BadRequest("Something Went Wrong");
			}
		}
		[HttpPut("delete/{id}")]
		public async Task<IActionResult> Delete_Information(int id)
		{
			try
			{
				if (id == 0)
				{
					return BadRequest(new {message="Id Is null here"});
				}
				else
				{
					var informationtodelete = await _context.HeaderInformations.FirstOrDefaultAsync(x => x.Id == id);

					if(informationtodelete == null)
					{
						return NotFound();
					}
					else
					{
						informationtodelete.IsDelete = "Y";
						await _context.SaveChangesAsync();
						return Ok(new {message="Information deleted Successfully"});

					}
				}
			}
			catch (Exception)
			{
				return BadRequest(new {message="Something went Wrong"});
			}
		}
	}
}
