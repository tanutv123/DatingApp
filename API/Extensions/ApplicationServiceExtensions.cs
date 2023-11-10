using API.Data;
using API.Helpers;
using API.interfaces;
using API.Services;
using Microsoft.EntityFrameworkCore;

namespace API.Extensions
{
	public static class ApplicationServiceExtensions
	{
		public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
		{
			//Add DBContext
			services.AddDbContext<DataContext>(options =>
			{
				options.UseSqlite(config.GetConnectionString("DefaultConnection"));
			});
			//Add CORS
			services.AddCors();
			//Add Services
			services.AddScoped<ITokenService, TokenService>();
			services.AddScoped<IUserRepository, UserRepository>();
			services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
			services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));
			services.AddScoped<IPhotoService, PhotoService>();
			services.AddScoped<LogUserActivity>(); 
			return services;
		}
	}
}
