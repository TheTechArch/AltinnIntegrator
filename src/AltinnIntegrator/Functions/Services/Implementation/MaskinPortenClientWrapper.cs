using AltinnIntegrator.Functions.Config;
using AltinnIntegrator.Functions.Services.Interface;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace AltinnIntegrator.Functions.Services.Implementation
{
    public class MaskinportenClientWrapper : IMaskinPortenClientWrapper
    {
        /// <summary>
        /// Application logger 
        /// </summary>
        private readonly AltinnIntegratorSettings _settings;

        private readonly HttpClient _client;

        protected static ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationClientWrapper" /> class.
        /// </summary>
        public MaskinportenClientWrapper(IOptions<AltinnIntegratorSettings> altinnIntegratorSettings, ILogger<MaskinportenClientWrapper> logger, HttpClient httpClient)
        {
            _settings = altinnIntegratorSettings.Value;
            _logger = logger;
            httpClient.BaseAddress = new Uri(_settings.MaskinportenBaseAddress);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
            _client = httpClient;
        }

        /// <summary>
        /// Gets or sets the base address
        /// </summary>
        private string BaseAddress { get; set; }

        public async Task<string> PostToken(FormUrlEncodedContent bearer)
        {
            string token = string.Empty;
           
            HttpResponseMessage response = await _client.PostAsync("/token", bearer);

            if (response.IsSuccessStatusCode)
            {
                token = response.Content.ReadAsStringAsync().Result;
                return token;
            }
            else
            {
                string error = await response.Content.ReadAsStringAsync();
                _logger.LogError( @"Could not retrieve Token" + error);
            }

            return null;
            
        }
    }
}
