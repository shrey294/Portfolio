namespace Portfolio.Models.DTO
{
	public class AboutMeUpdateDto
	{
		public string Description { get; set; }
		public double Experience { get; set; }
		public IFormFile ImageFile { get; set; }
	}

}
