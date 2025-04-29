using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Common.Configuration
{
    public static class ServicePortsExtensions
    {
        public static IServiceCollection AddServicePorts(this IServiceCollection services)
        {
            services.AddSingleton<ServicePorts>();
            return services;
        }

        public static int GetServicePort(this ServicePorts ports, string serviceName)
        {
            return ports.GetAvailablePort(serviceName);
        }
    }
} 