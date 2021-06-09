using AltinnIntegrator.Config;
using AltinnIntegrator.Services.Interface;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace AltinnIntegrator.Services.Implementation
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
        public MaskinportenClientWrapper(IOptions<AltinnIntegratorSettings> altinnIntegratorSettings, ILogger logger, HttpClient httpClient)
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

        public bool PostToken(FormUrlEncodedContent bearer, out string token)
        {
            token = string.Empty;
           
            Task<HttpResponseMessage> response = _client.PostAsync("/", bearer);

            if (response.Result.IsSuccessStatusCode)
            {
                token = response.Result.Content.ReadAsStringAsync().Result;
                return true;
            }
            else
            {
                _logger.LogError( @"Could not retrieve Token");
            }

            return false;
            
        }
    }
}
