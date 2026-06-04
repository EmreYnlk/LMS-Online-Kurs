using LMSPlatform.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LMSPlatform.Filters
{
    public class JetonFiltresi : IAsyncActionFilter
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public JetonFiltresi(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context.Controller is Controller controller && context.HttpContext.User.Identity!.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(context.HttpContext.User);
                if (user != null)
                    controller.ViewBag.JetonMiktari = user.JetonMiktari;
            }
            await next();
        }
    }
}
