using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using AltinnIntegrator.Functions.Config;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using AltinnIntegrator.Functions.Extensions;
using AltinnIntegrator.Functions.Services.Interface;

namespace AltinnIntegrator.Functions.Services.Implementation
{
    public class AuthenticationClientWrapper : IAuthenticationClientWrapper
    {
        private readonly HttpClient _client;

        private readonly AltinnIntegratorSettings _settings;

        /// <summary>
        /// Gets or sets the base address
        /// </summary>
        private string BaseAddress { get; set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationClientWrapper" /> class.
        /// </summary>
        public AuthenticationClientWrapper(IOptions<AltinnIntegratorSettings> altinnIntegratorSettings, HttpClient httpClient)
        {
            _settings = altinnIntegratorSettings.Value;
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
            _client = httpClient;
        }

        public async Task<string> ConvertToken(string token)
        {
            string cmd = $@"{_settings.PlatformBaseUrl}authentication/api/v1/exchange/maskinporten?test={_settings.TestMode}";
            HttpResponseMessage response = await _client.GetAsync(token,cmd);

            if (response.IsSuccessStatusCode)
            {
                string jsonString = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<string>(jsonString);
            }
            else
            {
                return $@"Could not retrieve Altinn Token";
            }
        }
    }
}
