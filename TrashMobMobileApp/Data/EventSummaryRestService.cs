namespace TrashMobMobileApp.Data
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Threading.Tasks;
    using TrashMobMobileApp.Models;

    public class EventSummaryRestService : RestServiceBase, IEventSummaryRestService
    {
        private readonly string EventSummaryApi = TrashMobServiceUrlBase + "eventsummaries";

        public async Task<EventSummary> GetEventSummaryAsync(Guid eventId)
        {
            try
            {
                var httpRequestMessage = new HttpRequestMessage();

                httpRequestMessage = GetDefaultHeaders(httpRequestMessage);
                httpRequestMessage.Method = HttpMethod.Get;
                httpRequestMessage.RequestUri = new Uri(EventSummaryApi + "/" + eventId);

                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.SendAsync(httpRequestMessage);

                string content = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<EventSummary>(content);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
                throw;
            }
        }

        public async Task<EventSummary> UpdateEventSummaryAsync(EventSummary eventSummary)
        {
            try
            {
                var httpRequestMessage = new HttpRequestMessage();
                httpRequestMessage = GetDefaultHeaders(httpRequestMessage);
                httpRequestMessage.Method = HttpMethod.Put;

                httpRequestMessage.RequestUri = new Uri(EventSummaryApi);

                httpRequestMessage.Content = JsonContent.Create(eventSummary, typeof(EventSummary), null, SerializerOptions);

                HttpClient client = new HttpClient();
                _ = await client.SendAsync(httpRequestMessage);

                return await GetEventSummaryAsync(eventSummary.EventId);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
                throw;
            }
        }

        public async Task<EventSummary> AddEventSummaryAsync(EventSummary eventSummary)
        {
            try
            {
                var httpRequestMessage = new HttpRequestMessage();
                httpRequestMessage = GetDefaultHeaders(httpRequestMessage);
                httpRequestMessage.Method = HttpMethod.Post;

                httpRequestMessage.RequestUri = new Uri(EventSummaryApi);

                httpRequestMessage.Content = JsonContent.Create(eventSummary, typeof(EventSummary), null, SerializerOptions);

                HttpClient client = new HttpClient();
                _ = await client.SendAsync(httpRequestMessage);

                return await GetEventSummaryAsync(eventSummary.EventId);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
                throw;
            }
        }
    }
}
