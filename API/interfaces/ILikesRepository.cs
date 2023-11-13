using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.interfaces
{
	public interface ILikesRepository
	{
		Task<UserLike> GetUserLike(int sourceUserId, int targetUserId);
		Task<AppUser> GetUserWithLikes(int userId);
		//predicate here shows which one the users want us to show
		//Users that liked them or users that they liked
		Task<PagedList<LikeDto>> GetUserLikes(LikeParams likeParams);

	}
}
