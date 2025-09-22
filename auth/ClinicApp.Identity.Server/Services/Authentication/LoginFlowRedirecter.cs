﻿using ClinicApp.Identity.Server.Services.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

public static class LoginFlowResultExtensions
{
    public static IActionResult ToActionResult(this LoginResult result, PageModel page)
    {
        return result.status switch
        {
            LoginFlowStatus.LoginSucceed =>
                new RedirectResult(result.returnUrl ?? "~/"),

            LoginFlowStatus.RequireEmailConfirmation =>
                new RedirectResult(result.returnUrl ?? "/Account/Features/ConfirmEmail"),

            LoginFlowStatus.RequireProfileCompletion =>
                new RedirectResult(result.returnUrl ?? "/Account/CompleteRegistration/Index"),

            LoginFlowStatus.AccessDenied =>
                page.Forbid(),

            _ =>
                new RedirectResult("~/")
        };
    }
}
