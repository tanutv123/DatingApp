using API.interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
	public class ImagesController : BaseApiController
	{
		private readonly IPhotoService _photoService;

		public ImagesController(IPhotoService photoService)
        {
			_photoService = photoService;
		}
        [HttpPost]
		public async Task<IActionResult> AddImage(IFormFile file)
		{
			var result = await _photoService.AddPhotoAsync(file);
			return Ok(new {link = result.SecureUrl.ToString()});
		}
	}
}
