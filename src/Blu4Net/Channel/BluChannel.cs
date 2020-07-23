using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Blu4Net.Channel
{
    public class BluChannel
    {
        public Uri Endpoint { get; }

        public BluChannel(Uri endpoint)
        {
            Endpoint = endpoint ?? throw new ArgumentNullException(nameof(endpoint));
        }

        public async Task<T> Send<T>(string request)
        {
            var serializer = new XmlSerializer(typeof(T));

            using (var client = new HttpClient())
            {
                var requestUri = new Uri(Endpoint, request).ToString();
                using (var response = await client.SendAsync(new HttpRequestMessage(HttpMethod.Get, requestUri)))
                using (var stream = await response.Content.ReadAsStreamAsync())
                {
                    return (T)serializer.Deserialize(stream);
                }
            }
        }

        public async Task<Status> GetStatus()
        {
            return await Send<Status>("Status");
        }

        public async Task<SyncStatus> GetSyncStatus()
        {
            return await Send<SyncStatus>("SyncStatus");
        }

    }
}
