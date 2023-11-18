using API.DTOs;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.interfaces;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
	public class LikesController : BaseApiController
	{
		private readonly IUnitOfWork _unitOfWork;

		public LikesController(IUnitOfWork unitOfWork)
        {
			_unitOfWork = unitOfWork;
		}

		[HttpPost("{username}")]
		public async Task<ActionResult> AddLike(string username)
		{
			var sourceUserId = User.GetUserId();
			var targetUser = await _unitOfWork.UserRepository.GetUserByUserNameAsync(username);
			var sourceUser = await _unitOfWork.LikeRepository.GetUserWithLikes(sourceUserId);

			if (targetUser == null) return NotFound();
			if(sourceUser.UserName == username) return BadRequest();

			var userLike = await _unitOfWork.LikeRepository.GetUserLike(sourceUserId, targetUser.Id);
			if (userLike != null) return BadRequest("You are already like this user");
			userLike = new UserLike
			{
				SourceUserId = sourceUserId,
				TargetUserId = targetUser.Id
			};

			sourceUser.LikedUsers.Add(userLike);
			if(await _unitOfWork.Complete()) return Ok();

			return BadRequest("Failed to like user");
		}

		[HttpGet]
		public async Task<ActionResult<PagedList<LikeDto>>> GetUserLikes([FromQuery] LikeParams likeParams)
		{
			likeParams.UserId = User.GetUserId();
			var users = await _unitOfWork.LikeRepository.GetUserLikes(likeParams);
			Response.AddPaginationHeader(new PaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPage));
			return Ok(users);
		}
    }
}
