﻿using AltinnIntegrator.Functions.Config;
using AltinnIntegrator.Functions.Services.Interface;
using AltinnIntegrator.Functions.Services.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace AltinnIntegrator.Functions.Services.Implementation
{
    public class AuthenticationService: IAuthenticationService
    {
        private readonly KeyVaultSettings _keyVaultSettings;
        private readonly IKeyVaultService _keyVaultService;
        private readonly AltinnIntegratorSettings _altinnIntegratorSettings;
        private readonly IMaskinPortenClientWrapper _maskinportenClientWrapper;
        private readonly IAuthenticationClientWrapper _authenticationClientWrapper;
        private string _altinnToken;

        public AuthenticationService(
            IKeyVaultService keyVaultService, 
            IOptions<KeyVaultSettings> keyVaultSettings, 
            IOptions<AltinnIntegratorSettings> altinnIntegratorSettings,
            IMaskinPortenClientWrapper maskinPortenClientWrapper,
            IAuthenticationClientWrapper authenticationClientWrapper)
        {
            _keyVaultSettings = keyVaultSettings.Value;
            _altinnIntegratorSettings = altinnIntegratorSettings.Value;
            _keyVaultService = keyVaultService;
            _maskinportenClientWrapper = maskinPortenClientWrapper;
            _authenticationClientWrapper = authenticationClientWrapper;
        }


        public async Task<string> GetAltinnToken()
        {
            if(!string.IsNullOrEmpty(_altinnToken))
            {
                return _altinnToken;
            }

            string jwtAssertion = await GetJwtAssertion();

            FormUrlEncodedContent content = GetUrlEncodedContent(jwtAssertion);
            string maskinPortenToken = await _maskinportenClientWrapper.PostToken(content);
            
            if (!string.IsNullOrEmpty(maskinPortenToken))
            {
                var accessTokenObject = JsonConvert.DeserializeObject<JObject>(maskinPortenToken);

                string altinnToken = await _authenticationClientWrapper.ConvertToken(accessTokenObject.GetValue("access_token").ToString());

                return altinnToken;
            }
            

            return null;
        }

        private FormUrlEncodedContent GetUrlEncodedContent(string assertion)
        {
            FormUrlEncodedContent formContent = new FormUrlEncodedContent(new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("grant_type", "urn:ietf:params:oauth:grant-type:jwt-bearer"),
                new KeyValuePair<string, string>("assertion", assertion),
            });

            return formContent;
        }

        public async Task<string> GetJwtAssertion()
        {
            DateTimeOffset dateTimeOffset = new DateTimeOffset(DateTime.UtcNow);
            Guid clientId = new Guid(_altinnIntegratorSettings.MaskinPortenClientId);

            X509Certificate2 cert = GetCertificateFromKeyStore("111bd85e12ef3dce6ec7b2c0011d4e9cbc4f896f", StoreName.My, StoreLocation.LocalMachine);

            X509SecurityKey securityKey = new X509SecurityKey(cert);
            JwtHeader header = new JwtHeader(new SigningCredentials(securityKey, SecurityAlgorithms.RsaSha256))
            {
                {"x5c", new List<string>() {Convert.ToBase64String(cert.GetRawCertData())}}
            };
            header.Remove("typ");
            header.Remove("kid");

            JwtPayload payload = new JwtPayload
            {
                { "aud", "https://ver2.maskinporten.no/" },
                { "resource", "https://tt02.altinn.no/maskinporten-api/" },
                { "scope", "altinn:serviceowner/instances.read altinn:serviceowner/instances.write" },
                { "iss", clientId },
                { "exp", dateTimeOffset.ToUnixTimeSeconds() + 10 },
                { "iat", dateTimeOffset.ToUnixTimeSeconds() },
                { "jti", Guid.NewGuid().ToString() },
            };

            JwtSecurityToken securityToken = new JwtSecurityToken(header, payload);
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();

            return handler.WriteToken(securityToken);
        }

        private async Task<X509Certificate2> GetCertificateFromKeyVault()
        {
            string certBase64 = await _keyVaultService.GetCertificateAsync(_keyVaultSettings.KeyVaultURI, _keyVaultSettings.MaskinPortenCertSecretId);
            return new X509Certificate2(Convert.FromBase64String(certBase64), (string)null, X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet | X509KeyStorageFlags.Exportable);
       }



        private static X509Certificate2 GetCertificateFromKeyStore(string thumbprint, StoreName storeName, StoreLocation storeLocation, bool onlyValid = false)
        {
            var store = new X509Store(storeName, storeLocation);
            store.Open(OpenFlags.ReadOnly);
            var certCollection = store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, onlyValid);
            var enumerator = certCollection.GetEnumerator();
            X509Certificate2 cert = null;
            while (enumerator.MoveNext())
            {
                cert = enumerator.Current;
            }

            return cert;
        }

    }
}
