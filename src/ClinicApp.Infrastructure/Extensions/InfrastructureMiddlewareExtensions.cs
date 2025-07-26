namespace ClinicApp.Infrastructure.Extensions;

using ClinicApp.Infrastructure.Middlewares;
using Microsoft.AspNetCore.Builder;

public static class InfrastructureMiddlewareExtensions
{
    public static IApplicationBuilder  UseInfrastructure(this IApplicationBuilder app)
    {
        app.UseMiddleware<EventualConsistencyMiddleware>();
        return app;
    }
}

