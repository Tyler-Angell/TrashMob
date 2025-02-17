﻿namespace TrashMobMobileApp.Data
{
    using System.Net.Http;
    using System.Text.Json;
    using System.Threading.Tasks;
    using TrashMobMobileApp.Authentication;

    public class RestServiceBase
    {
        protected JsonSerializerOptions SerializerOptions { get; private set; }
        
        public HttpClientService HttpClientService { get; }

        private readonly IB2CAuthenticationService b2CAuthenticationService;

        protected RestServiceBase(HttpClientService httpClientService, IB2CAuthenticationService b2CAuthenticationService)
        {
            SerializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true
            };
            HttpClientService = httpClientService;
            this.b2CAuthenticationService = b2CAuthenticationService;
        }
    }
}