using API.Entities;
using API.interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
	public class AdminController : BaseApiController
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly IUnitOfWork _unitOfWork;

		public AdminController(UserManager<AppUser> userManager, 
			IUnitOfWork unitOfWork)
        {
			_userManager = userManager;
			_unitOfWork = unitOfWork;
		}
        [Authorize(Policy = "RequireAdminRole")]
		[HttpGet("users-with-roles")]
		public async Task<ActionResult> GetUsersWithRoles()
		{
			var users = await _userManager.Users
				.OrderBy(u => u.UserName)
				.Select(u => new
				{
					u.Id,
					UserName = u.UserName,
					Roles = u.UserRoles.Select(u => u.Role.Name).ToList()
				}).ToListAsync();
			return Ok(users);
		}

		[Authorize(Policy ="RequireAdminRole")]
		[HttpPost("edit-roles/{username}")]
		public async Task<ActionResult> EditRoles(string username, [FromQuery]string roles)
		{
			if(string.IsNullOrEmpty(roles)) return BadRequest("You did not select roles for user");

			var selectedRoles = roles.Split(',').ToArray();

			var user = await _userManager.FindByNameAsync(username);

			if (user == null) return NotFound();

			var userRoles = await _userManager.GetRolesAsync(user);

			var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));

			if (!result.Succeeded) return BadRequest("Failed to add roles");

			result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));

			if (!result.Succeeded) return BadRequest("Failed to remove roles");

			return Ok(await _userManager.GetRolesAsync(user));	
		}

		[Authorize(Policy = "ModeratorPhotoRole")]
		[HttpGet("photos-to-moderate")]
		public ActionResult GetPhotosForModeration() 
		{
			return Ok("Only admin and moderator can see this");
		}

		[Authorize(Policy = "ModeratorPhotoRole")]
		[HttpPut("restrict-user/{username}")]
		public async Task<ActionResult> RestrictUser(string username)
		{
			await _unitOfWork.UserRepository.RestrictUser(username);
			if (await _unitOfWork.Complete()) return Ok();
			return BadRequest("Problem in restricting user(maybe user is already restricted)");
		}
	}
}
