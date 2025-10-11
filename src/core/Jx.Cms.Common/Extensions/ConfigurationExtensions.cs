using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Jx.Cms.Common.Extensions
{
    public static class ConfigurationExtensions
    {
        public static IServiceCollection AddConfigurableOptions<T>(this IServiceCollection services, IConfiguration configuration = null) where T : class, new()
        {
            if (configuration != null)
            {
                services.Configure<T>(configuration.GetSection(typeof(T).Name));
            }
            else
            {
                services.AddSingleton<IPostConfigureOptions<T>, PostConfigureOptions<T>>();
                services.AddOptions<T>().Configure<IConfiguration>((config, cfg) => cfg.GetSection(typeof(T).Name).Bind(config));
            }
            return services;
        }
    }
}