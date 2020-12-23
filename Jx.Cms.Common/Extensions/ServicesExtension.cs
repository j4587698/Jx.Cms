using System.Linq;
using Microsoft.Extensions.DependencyInjection;

namespace Jx.Cms.Common.Extensions
{
    public static class ServicesExtension
    {
        public static IServiceCollection Services { get; set; }
        
        public static T GetSingletonInstanceOrNull<T>(this IServiceCollection services)
        {
            ServiceDescriptor descriptor = services.FirstOrDefault(d => d.ServiceType == typeof(T) && d.Lifetime == ServiceLifetime.Singleton);

            if (descriptor?.ImplementationInstance != null)
            {
                return (T)descriptor.ImplementationInstance;
            }

            if (descriptor?.ImplementationFactory != null)
            {
                return (T)descriptor.ImplementationFactory.Invoke(null);
            }

            return default;
        }
    }
}