﻿using API.Data;
using API.DTOs;
using API.Entities;
using API.interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
	public class AccountController : BaseApiController
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly DataContext _dataContext;
		private readonly ITokenService _tokenService;
		private readonly IMapper _mapper;

		public AccountController(UserManager<AppUser> userManager, DataContext dataContext, ITokenService tokenService, IMapper mapper)
        {
			_userManager = userManager;
			_dataContext = dataContext;
			_tokenService = tokenService;
			_mapper = mapper;
		}

		[HttpPost("register")]
		public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
		{
			if( !(await UserExist(registerDto.UserName)))
			{
				var user = _mapper.Map<AppUser>(registerDto);
				user.UserName = user.UserName.ToLower();
				var result = await _userManager.CreateAsync(user, registerDto.Password);
				if (!result.Succeeded) return BadRequest(result.Errors);

				var roleResult = await _userManager.AddToRoleAsync(user, "Member");
				if (!roleResult.Succeeded) return BadRequest(result.Errors);

				return new UserDto
				{
					UserName = user.UserName,
					Token = await _tokenService.CreateToken(user),
					KnownAs = user.KnownAs,
					Gender = user.Gender
				};
			} else
			{
				return BadRequest("Username is taken");
			}
			
			
		}

		[HttpPost("login")]
		public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
		{
			var user = await _userManager.Users
				.Include(p => p.Photos)
				.SingleOrDefaultAsync(x => x.UserName.Equals(loginDto.Username));
            if (user == null) return Unauthorized("Invalid Username");
			var result = await _userManager.CheckPasswordAsync(user, loginDto.Password);
			if (!result) return Unauthorized("Invalid username or password");
			if(user.IsRestricted) return BadRequest("Your account are restricted! Contact support team for further details");
			return new UserDto
			{
				UserName = user.UserName,
				Token = await _tokenService.CreateToken(user),
				PhotoUrl = user.Photos.FirstOrDefault(x => x.isMain)?.Url,
				KnownAs = user.KnownAs,
				Gender = user.Gender
			};

		}
        [HttpPost("loginAsTodd")]
        public async Task<ActionResult<UserDto>> LoginAsTodd()
        {
            var user = await _userManager.Users
                .Include(p => p.Photos)
                .SingleOrDefaultAsync(x => x.UserName.Equals("todd"));
            if (user == null) return Unauthorized("Invalid Username");
            var result = await _userManager.CheckPasswordAsync(user, "Pa$$w0rd");
            if (!result) return Unauthorized("Invalid username or password");
            if (user.IsRestricted) return BadRequest("Your account are restricted! Contact support team for further details");
            return new UserDto
            {
                UserName = user.UserName,
                Token = await _tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.isMain)?.Url,
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };

        }

        private async Task<bool> UserExist(string username)
		{
			return await _userManager.Users.AnyAsync(x => x.UserName.Equals(username.ToLower()));
		}
    }
}
