﻿namespace TrashMobMobileApp.Data
{
    using Newtonsoft.Json;
    using System;
    using System.Diagnostics;
    using System.Net.Http;
    using System.Threading.Tasks;
    using TrashMobMobileApp.Models;

    public class MapRestService : RestServiceBase, IMapRestService
    {
        private readonly string MapsApi = TrashMobServiceUrlBase + "maps";

        public async Task<Address> GetAddressAsync(double latitude, double longitude)
        {
            Address address = new Address();
            try
            {
                var httpRequestMessage = new HttpRequestMessage();

                httpRequestMessage = GetDefaultHeaders(httpRequestMessage);
                httpRequestMessage.Method = HttpMethod.Get;
                httpRequestMessage.RequestUri = new Uri(MapsApi + $"/GetAddress?latitude={latitude}&longitude={longitude}");

                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.SendAsync(httpRequestMessage);

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    address = JsonConvert.DeserializeObject<Address>(content);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }

            return address;
        }
    }
}
