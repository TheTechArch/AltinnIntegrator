using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using AltinnIntegrator.Config;
using AltinnIntegrator.Services.Interface;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using AltinnIntegrator.Extensions;

namespace AltinnIntegrator.Services.Implementation
{
    class AuthenticationClientWrapper : IAuthenticationClientWrapper
    {

        private readonly HttpClient _client;


        private readonly AltinnIntegratorSettings _settings;

        /// <summary>
        /// Application logger 
        /// </summary>
        protected static ILogger _logger;

        /// <summary>
        /// Gets or sets the base address
        /// </summary>
        private string BaseAddress { get; set; }


        /// <summary>
        /// Initializes a new instance of the <see cref="AuthenticationClientWrapper" /> class.
        /// </summary>
        public AuthenticationClientWrapper(IOptions<AltinnIntegratorSettings> altinnIntegratorSettings, ILogger logger, HttpClient httpClient)
        {
            _settings = altinnIntegratorSettings.Value;
            _logger = logger;
            httpClient.BaseAddress = new Uri(_settings.MaskinportenBaseAddress);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
            _client = httpClient;
        }

        public async Task<string> ConvertToken(string token)
        {
            AuthenticationHeaderValue headers = new AuthenticationHeaderValue("Bearer", token);

            string cmd = $@"exchange/maskinporten?test={_settings.TestMode}";
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
