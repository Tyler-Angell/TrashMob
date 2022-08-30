﻿namespace TrashMobMobileApp.Data
{
    using System;
    using System.Diagnostics;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Threading.Tasks;
    using TrashMobMobileApp.Models;

    public class ContactRequestRestService : RestServiceBase, IContactRequestRestService
    {
        private readonly Uri ContactRequestApi = new Uri(TrashMobServiceUrlBase + "contactrequest");

        public async Task AddContactRequest(ContactRequest contactRequest)
        {
            try
            {
                contactRequest.Id = Guid.NewGuid().ToString();

                var httpRequestMessage = new HttpRequestMessage();
                httpRequestMessage = GetDefaultHeaders(httpRequestMessage);
                httpRequestMessage.Method = HttpMethod.Post;
                httpRequestMessage.RequestUri = ContactRequestApi;

                httpRequestMessage.Content = JsonContent.Create(contactRequest, typeof(ContactRequest), null, SerializerOptions);

                HttpClient client = new HttpClient();
                HttpResponseMessage response = await client.SendAsync(httpRequestMessage);

                // Handle message sent successfully
            }
            catch (Exception ex)
            {
                Debug.WriteLine(@"\tERROR {0}", ex.Message);
            }
        }
    }
}
