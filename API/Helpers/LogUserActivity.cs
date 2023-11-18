using API.Extensions;
using API.interfaces;
using Microsoft.AspNetCore.Mvc.Filters;

namespace API.Helpers
{
	public class LogUserActivity : IAsyncActionFilter
	{
		//This method is for logging the user and updating the LastActive value of that user in the database
		public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
		{
			var resultContext = await next();


			//This is not nessessary because this is called inside a method that has [Authorized] attribute on it
			//However, if someone magically can get access to this method, we can be clear about that.
			if (!resultContext.HttpContext.User.Identity.IsAuthenticated) return;

			var userId = resultContext.HttpContext.User.GetUserId();

			var uow = resultContext.HttpContext.RequestServices.GetRequiredService<IUnitOfWork>();
			var user = await uow.UserRepository.GetUserByIdAsync(userId);
			user.LastActive = DateTime.UtcNow;
			await uow.Complete();
		}
	}
}
