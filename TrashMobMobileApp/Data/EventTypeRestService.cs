﻿namespace TrashMobMobileApp.Data
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Net.Http;
    using System.Threading.Tasks;
    using TrashMobMobileApp.Models;

    public class EventTypeRestService : RestServiceBase, IEventTypeRestService
    {
        private readonly string EventTypesApi = TrashMobServiceUrlBase + "eventtypes";

        public async Task<IEnumerable<EventType>> GetEventTypesAsync()
        {
            var eventTypes = new List<EventType>();

            try
            {
                var httpRequestMessage = new HttpRequestMessage();

                httpRequestMessage = GetDefaultHeaders(httpRequestMessage);
                httpRequestMessage.Method = HttpMethod.Get;
                httpRequestMessage.RequestUri = new Uri(EventTypesApi);

                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.SendAsync(httpRequestMessage);

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    eventTypes = JsonConvert.DeserializeObject<List<EventType>>(content);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }

            return eventTypes;
        }
    }
}
