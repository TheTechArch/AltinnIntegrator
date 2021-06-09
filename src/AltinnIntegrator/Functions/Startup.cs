using AltinnIntegrator.Functions.Config;
using AltinnIntegrator.Functions;
using AltinnIntegrator.Functions.Services.Implementation;
using AltinnIntegrator.Functions.Services.Interface;
using AltinnIntegrator.Functions.Services.Interfaces;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: WebJobsStartup(typeof(Startup))]

namespace AltinnIntegrator.Functions
{
    /// <summary>
    /// Function events startup
    /// </summary>
    public class Startup : IWebJobsStartup
    {
        /// <summary>
        /// Gets functions project configuration
        /// </summary>
        public void Configure(IWebJobsBuilder builder)
        {
            builder.Services.AddOptions<AltinnIntegratorSettings>()
            .Configure<IConfiguration>((settings, configuration) =>
            {
                configuration.GetSection("AltinnIntegrator").Bind(settings);
            });
            builder.Services.AddOptions<KeyVaultSettings>()
            .Configure<IConfiguration>((settings, configuration) =>
            {
                configuration.GetSection("KeyVault").Bind(settings);
            });
               
            builder.Services.AddSingleton<ITelemetryInitializer, TelemetryInitializer>();
            builder.Services.AddSingleton<IKeyVaultService, KeyVaultService>();
            builder.Services.AddSingleton<IAuthenticationClientWrapper, AuthenticationClientWrapper>();
            builder.Services.AddHttpClient<IAuthenticationClientWrapper, AuthenticationClientWrapper>();
            builder.Services.AddHttpClient<IMaskinPortenClientWrapper, MaskinportenClientWrapper>();
        }
    }
}
