using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Altinn.Platform.Storage.Interface.Models;
using AltinnIntegrator.Config;
using AltinnIntegrator.Extensions;
using AltinnIntegrator.Services.Interface;
using AltinnIntegrator.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Newtonsoft.Json;

namespace AltinnIntegrator.Services.Implementation
{
    /// <summary>
    /// App implementation of the instance service that talks to platform storage.
    /// </summary>
    public class AltinnAppSI : IAltinnApp
    {
        private readonly ILogger _logger;
         private readonly HttpClient _client;
        private readonly AltinnIntegratorSettings _settings;
        private readonly IAuthenticationService _authenticationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="InstanceAppSI"/> class.
        /// </summary>
        /// <param name="platformSettings">the platform settings</param>
        /// <param name="logger">the logger</param>
        /// <param name="httpClient">A HttpClient that can be used to perform HTTP requests against the platform.</param>
        /// <param name="settings">The application settings.</param>
        public AltinnAppSI(IOptions<AltinnIntegratorSettings> altinnIntegratorSettings, HttpClient httpClient, IAuthenticationService authenticationService)
        {
            _settings = altinnIntegratorSettings.Value;
            httpClient.BaseAddress = new Uri(_settings.AppsBaseUrl);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/xml"));
            _client = httpClient;
            _authenticationService = authenticationService;
        }

        /// <inheritdoc />
        public async Task<Instance> GetInstance(string app, string org, int instanceOwnerId, Guid instanceGuid)
        {
            string instanceIdentifier = $"{instanceOwnerId}/{instanceGuid}";

            string apiUrl = $"instances/{instanceIdentifier}";

            string altinnToken = await _authenticationService.GetAltinnToken();
            
            HttpResponseMessage response = await _client.GetAsync(altinnToken, apiUrl);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                string instanceData = await response.Content.ReadAsStringAsync();
                Instance instance = JsonConvert.DeserializeObject<Instance>(instanceData);
                return instance;
            }
            else
            {
                _logger.LogError($"Unable to fetch instance with instance id {instanceGuid}");
                throw new ApplicationException();
            }
        }

        /// <inheritdoc />
        public async Task<Instance> GetInstance(Instance instance)
        {
            string app = instance.AppId.Split("/")[1];
            string org = instance.Org;
            int instanceOwnerId = int.Parse(instance.InstanceOwner.PartyId);
            Guid instanceGuid = Guid.Parse(instance.Id.Split("/")[1]);

            return await GetInstance(app, org, instanceOwnerId, instanceGuid);
        }
     
        /// <inheritdoc/>
        public async Task<Instance> AddCompleteConfirmation(int instanceOwnerPartyId, Guid instanceGuid)
        {
            string apiUrl = $"instances/{instanceOwnerPartyId}/{instanceGuid}/complete";
            string altinnToken = await _authenticationService.GetAltinnToken(); 

            HttpResponseMessage response = await _client.PostAsync(altinnToken, apiUrl, new StringContent(string.Empty));

            if (response.StatusCode == HttpStatusCode.OK)
            {
                string instanceData = await response.Content.ReadAsStringAsync();
                Instance instance = JsonConvert.DeserializeObject<Instance>(instanceData);
                return instance;
            }

            throw new ApplicationException();
        }

        public Task<Instance> AddCompleteConfirmation(string app, string org, int instanceOwnerPartyId, Guid instanceGuid)
        {
            throw new NotImplementedException();
        }
    }
}
