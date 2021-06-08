using AltinnIntegrator.Config;
using AltinnIntegrator.Services.Interface;
using Microsoft.Extensions.Options;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace AltinnIntegrator.Services.Implementation
{
    /// <summary>
    /// Service that downloads data from platform
    /// </summary>
    public class PlatformSI : IPlatform
    {
        private readonly HttpClient _client;
        private readonly AltinnIntegratorSettings _settings;

        public PlatformSI(IOptions<AltinnIntegratorSettings> altinnIntegratorSettings,
            HttpClient httpClient)
        {
            _settings = altinnIntegratorSettings.Value;
            httpClient.BaseAddress = new Uri(_settings.PlatformBaseUrl);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
            _client = httpClient;
        }


        public Task<Stream> GetBinaryData(string org, string app, int instanceOwnerPartyId, Guid instanceGuid, Guid dataId)
        {
            throw new NotImplementedException();
        }
    }
}
