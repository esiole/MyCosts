using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MyCosts.Api.Extensions;
using MyCosts.Application.Services;

namespace MyCosts.Api.ActionFilters;

public class UserFilter : IAsyncActionFilter
{
    private readonly IUserService _userService;

    public UserFilter(IUserService userService)
    {
        _userService = userService;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var userId = context.HttpContext.User.GetUserId();
        var user = await _userService.GetAsync(userId);

        if (user != null)
        {
            context.HttpContext.Items.Add("User", user);
            await next();
        }

        context.Result = new UnauthorizedResult();
    }
}