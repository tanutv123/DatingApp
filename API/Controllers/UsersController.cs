using API.Data;
using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
	[Authorize]
	public class UsersController : BaseApiController
	{
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;
		private readonly IPhotoService _photoService;

		public UsersController(IUnitOfWork unitOfWork, IMapper mapper, IPhotoService photoService)
        {
			_unitOfWork = unitOfWork;
			_mapper = mapper;
			_photoService = photoService;
		}

		[HttpGet]
		public async Task<ActionResult<PagedList<MemberDto>>> GetUsers([FromQuery]UserParams userParams)
		{
			var currentUsername = User.GetUserName();
			var gender = await _unitOfWork.UserRepository.GetUserGender(currentUsername);
			if (gender == null) return NotFound();
			userParams.CurrentUsername = currentUsername;
			if(string.IsNullOrEmpty(userParams.Gender))
			{
				userParams.Gender = gender == "male" ? "female" : "male";
			}
			if (userParams.minAge > userParams.maxAge)
			{
				return BadRequest("Minimun Age cannot exceed Maximum Age");
			}
			var users = await _unitOfWork.UserRepository.GetMembersAsync(userParams);
			Response.AddPaginationHeader(new PaginationHeader(users.CurrentPage,
																users.PageSize, 
																users.TotalCount, 
																users.TotalPage));
			return Ok(users);
			
		}

		[HttpGet("{username}")]
		public async Task<ActionResult<MemberDto>> GetUser(string username)
		{
			return  await _unitOfWork.UserRepository.GetMemberAsync(username);
			
		}

		[HttpPut]
		public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
		{
			var user = await _unitOfWork.UserRepository.GetUserByUserNameAsync(User.GetUserName());

			if (user == null) return NotFound();

			_mapper.Map(memberUpdateDto, user);
			if (await _unitOfWork.Complete()) return NoContent();

			return BadRequest("Failed to update user");
		}

		[HttpPost("add-photo")]
		public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
		{
			var user = await _unitOfWork.UserRepository.GetUserByUserNameAsync(User.GetUserName());
			if (user == null) return NotFound();

			var result = await _photoService.AddPhotoAsync(file);
			if (result.Error != null) return BadRequest(result.Error.Message);

			var photo = new Photo
			{
				Url = result.SecureUrl.AbsoluteUri,
				PublicId = result.PublicId
			};

			if(user.Photos.Count == 0 ) photo.isMain = true;
			user.Photos.Add(photo);

			if (await _unitOfWork.Complete())
			{
				return CreatedAtAction(nameof(GetUser), 
										new {username = user.UserName}, 
										_mapper.Map<PhotoDto>(photo));
			}
			return BadRequest("Problem adding photo");

		}
		[HttpPut("set-main-photo/{photoId}")]
		public async Task<ActionResult> SetMainPhoto(int photoId)
		{
			var user = await _unitOfWork.UserRepository.GetUserByUserNameAsync(User.GetUserName());
			if (user == null) return NotFound("User is not found");

			var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);
			if (photo == null) return NotFound("Photo not found");
			if (photo.isMain) return BadRequest("This is not your main photo");

			var currentMain = user.Photos.FirstOrDefault( x=> x.isMain);
			if (currentMain != null) currentMain.isMain = false;

			photo.isMain = true;
			if (await _unitOfWork.Complete()) return NoContent();
			return BadRequest("Problem at setting main photo");

		}

		[HttpDelete("delete-photo/{photoId}")]
		public async Task<ActionResult> DeletePhoto(int photoId)
		{
			var user = await _unitOfWork.UserRepository.GetUserByUserNameAsync(User.GetUserName());

			var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);
			if (photo == null) return NotFound("Photo not found");

			if (photo.isMain) return BadRequest("You cannot delete your main photo");

			if (photo.PublicId != null)
			{
				var result = await _photoService.DeletePhotoAsync(photo.PublicId);
				if(result.Error != null) return BadRequest($"{result.Error.Message}");
			}

			user.Photos.Remove(photo);
			if (await _unitOfWork.Complete()) { return Ok(); }
			return BadRequest("Problem in deleting photo");
		}



		
    }
}
