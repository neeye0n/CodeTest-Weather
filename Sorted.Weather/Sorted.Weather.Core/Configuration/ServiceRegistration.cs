using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Sorted.Weather.Core.Rainfall.Handlers;
using Sorted.Weather.Core.Rainfall.Validation;
using Sorted.Weather.Core.Services;
using System.Reflection;

namespace Sorted.Weather.Core.Configuration
{
    public static class ServiceRegistration
    {
        public static IServiceCollection RegisterCoreServices(this IServiceCollection services, ServiceConfiguration config)
        {
            // Check Core Service Configs
            if (config.WeatherServiceApiUrl is null)
            {
                throw new InvalidOperationException("Invalid configuration. Cannot proceed.");
            }

            services.AddHttpClient("WeatherClient", client =>
            {
                client.BaseAddress = new Uri(config.WeatherServiceApiUrl);
            });
            services.AddScoped<IWeatherService, WeatherService>();
            services.AddMediatR(config => config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
            services.AddValidatorsFromAssemblyContaining<GetRainfallReadingsValidator>();
            return services;
        }
    }
}
