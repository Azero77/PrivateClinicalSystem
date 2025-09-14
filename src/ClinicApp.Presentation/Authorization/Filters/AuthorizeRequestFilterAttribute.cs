using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;
using System.Security.Claims;

namespace ClinicApp.Presentation.Authorization.Filters;


/// <summary>
/// Filter to run the Authorization by passing the request as the resource to check some authorization constraints against the request
/// </summary>
public sealed class AuthorizeByRequestFilter<TRequirement,TRequest> : IAsyncActionFilter
    where TRequirement : IAuthorizationRequirement,new()
{
    private readonly IAuthorizationService _authorizationService;

    public AuthorizeByRequestFilter(IAuthorizationService authorizationService)
    {
        _authorizationService = authorizationService;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var user = context.HttpContext.User;
        var userId = user.FindFirst(ClaimTypes.NameIdentifier);
        var requirement = new TRequirement();
        var model = context.ActionArguments.Values.OfType<TRequest>().FirstOrDefault();
        if (model is null)
            Log.Error("You are Providing Authorization For Action and the request is {@RequestName} is not matching...",new { typeof(TRequest).Name});
        var result = await _authorizationService.AuthorizeAsync(user, model, requirement);
        if (!result.Succeeded)
        {
            context.Result = new ForbidResult();return;
        }
        await next();
    }
}

public class AuthorizeByRequestFilterAttribute<TRequirement, TRequest> :
    TypeFilterAttribute<AuthorizeByRequestFilter<TRequirement, TRequest>>
    where TRequirement : IAuthorizationRequirement, new()
{
    public AuthorizeByRequestFilterAttribute() : base()
    {
    }
}