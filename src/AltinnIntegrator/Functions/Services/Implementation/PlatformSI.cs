using AltinnIntegrator.Functions.Config;
using AltinnIntegrator.Functions.Extensions;
using AltinnIntegrator.Functions.Services.Interface;
using AltinnIntegrator.Functions.Services.Interfaces;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace AltinnIntegrator.Functions.Services.Implementation
{
    /// <summary>
    /// Service that downloads data from platform
    /// </summary>
    public class PlatformSI : IPlatform
    {
        private readonly HttpClient _client;
        private readonly AltinnIntegratorSettings _settings;
        private readonly IAuthenticationService _authenticationService;

        public PlatformSI(IOptions<AltinnIntegratorSettings> altinnIntegratorSettings,
            HttpClient httpClient, IAuthenticationService authenticationService)
        {
            _settings = altinnIntegratorSettings.Value;
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
            _client = httpClient;
            _authenticationService = authenticationService;
        }


        public async Task<Stream> GetBinaryData(string dataUri)
        {
            string altinnToken = await _authenticationService.GetAltinnToken();

            HttpResponseMessage response = await _client.GetAsync(altinnToken, dataUri);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadAsStreamAsync();
            }
            else if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return null;
            }

            throw new ApplicationException();
        }
    }
}
