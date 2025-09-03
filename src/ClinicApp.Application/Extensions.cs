using Castle.DynamicProxy;
using Microsoft.Extensions.DependencyInjection;

namespace ClinicApp.Application;
public static class Extensions
{
    private static readonly ProxyGenerator _proxyGenerator = new();
    /// <summary>
    /// For adding Interceptors to a repo
    /// </summary>
    public static void AddProxidScoped<TInterface, TImplementation, TInterceptor>(this IServiceCollection services)
        where TInterface : class
        where TImplementation : class,TInterface
        where TInterceptor : IInterceptor
    {
        services.AddScoped<TImplementation>();
        services.AddScoped(sp =>
        {
            var implementation = sp.GetRequiredService<TImplementation>();
            var interceptor = sp.GetRequiredService<TInterceptor>();
            return _proxyGenerator.CreateInterfaceProxyWithTarget<TInterface>(implementation,interceptor);
        });
    }
}
