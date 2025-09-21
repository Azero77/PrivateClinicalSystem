using ClinicApp.Identity.Presentation;
using Duende.Bff.Yarp;
using Microsoft.AspNetCore.Authentication;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddBff()
    .AddRemoteApis();

Configuration config = new();
builder.Configuration.Bind("BFF", config);

var isDev = builder.Environment.IsDevelopment();

builder.Services.AddAuthentication();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultScheme = "cookie";
        options.DefaultChallengeScheme = "oidc";
        options.DefaultSignOutScheme = "oidc";
    })
    .AddCookie("cookie", options =>
    {
        options.Cookie.Name = isDev ? "bff-dev" : "__Host-bff";

        options.Cookie.SameSite = SameSiteMode.None;
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;

    })
    .AddOpenIdConnect("oidc", options =>
    {
        if (isDev)
        {
            options.RequireHttpsMetadata = false;
        }
        options.Authority = config.Authority;
        options.ClientId = config.ClientId;
        options.ClientSecret = config.ClientSecret;
        options.RequireHttpsMetadata = !isDev; //when dev we use http
        options.ResponseType = "code";
        options.ResponseMode = "query";

        options.GetClaimsFromUserInfoEndpoint = true;
        options.MapInboundClaims = false;
        options.SaveTokens = true;

        options.Scope.Clear();
        foreach (var scope in config.Scopes)
        {
            options.Scope.Add(scope);
        }

        options.TokenValidationParameters = new()
        {
            NameClaimType = "name",
            RoleClaimType = "role"
        };
    });


var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

app.UseAuthentication();
app.UseBff();

app.MapBffManagementEndpoints();

if (config.Apis.Any())
{
    foreach (var api in config.Apis)
    {
        app.MapRemoteBffApiEndpoint(api.LocalPath, api.RemoteUrl!)
            .RequireAccessToken(api.RequiredToken);
    }
}
app.MapGet("/credentials", async (HttpContext context) =>
{
    var accessToken = await context.GetTokenAsync("access_token");
    var idToken = await context.GetTokenAsync("id_token");
    var refreshToken = await context.GetTokenAsync("refresh_token");
    return new { accessToken, idToken, refreshToken };
});

app.Run();
