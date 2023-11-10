using API.Data;
using API.DTOs;
using API.Entities;
using API.interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace API.Controllers
{
	public class AccountController : BaseApiController
	{
		private readonly DataContext _dataContext;
		private readonly ITokenService _tokenService;
		private readonly IMapper _mapper;

		public AccountController(DataContext dataContext, ITokenService tokenService, IMapper mapper)
        {
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
				using var hmac = new HMACSHA512();
				user.UserName = registerDto.UserName.ToLower();
				user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password));
				user.PasswordSalt = hmac.Key;
				_dataContext.Users.Add(user);
				await _dataContext.SaveChangesAsync();
				return new UserDto
				{
					UserName = user.UserName,
					Token = _tokenService.CreateToken(user),
					KnownAs = user.KnownAs,
					Gender = user.Gender,
				};
			} else
			{
				return BadRequest("Username is taken");
			}
			
			
		}

		[HttpPost("login")]
		public async Task<ActionResult<UserDto>> Login(LoginDto loginDto)
		{
			var user = await _dataContext.Users
				.Include(p => p.Photos)
				.SingleOrDefaultAsync(x => x.UserName.Equals(loginDto.Username));
            if (user == null)
			{
				return Unauthorized("Invalid Username");
			}
			using var hmac = new HMACSHA512(user.PasswordSalt);
			var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDto.Password));

			for(int i = 0; i < computedHash.Length; i++)
			{
				if (computedHash[i] != user.PasswordHash[i]) return Unauthorized("Invalid Password");
			}

			return new UserDto
			{
				UserName = user.UserName,
				Token = _tokenService.CreateToken(user),
				PhotoUrl = user.Photos.FirstOrDefault(x => x.isMain)?.Url,
				KnownAs = user.KnownAs,
				Gender = user.Gender
			};

		}

		private async Task<bool> UserExist(string username)
		{
			return await _dataContext.Users.AnyAsync(x => x.UserName.Equals(username.ToLower()));
		}
    }
}
