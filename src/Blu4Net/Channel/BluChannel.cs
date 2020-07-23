using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net.Http;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Blu4Net.Channel
{
    public class BluChannel
    {
        public Uri Endpoint { get; }
        public TimeSpan Timeout { get; } = TimeSpan.FromSeconds(5);

        public BluChannel(Uri endpoint)
        {
            Endpoint = endpoint ?? throw new ArgumentNullException(nameof(endpoint));
        }

        private async Task<XDocument> SendRequest(string request, NameValueCollection parameters, TimeSpan timeout, CancellationToken cancellationToken)
        {
            var requestUri = new UriBuilder(Endpoint)
            {
                Path = request,
                Query = parameters?.ToString(),
            }.Uri;

            using (var client = new HttpClient() { Timeout = timeout } )
            {
                using (var response = await client.GetAsync(requestUri, cancellationToken))
                using (var stream = await response.Content.ReadAsStreamAsync())
                {
                    return XDocument.Load(stream);
                }
            }
        }

        private Task<XDocument> SendRequest(string request, NameValueCollection parameters = null)
        {
            return SendRequest(request, parameters, Timeout, CancellationToken.None);
        }

        private Task<XDocument> SendRequest(string request, NameValueCollection parameters, CancellationToken cancellationToken)
        {
            return SendRequest(request, parameters, TimeSpan.FromMilliseconds(System.Threading.Timeout.Infinite), cancellationToken);
        }

        public async Task<T> SendRequest<T>(string request, NameValueCollection parameters = null)
        {
            var response = await SendRequest(request, parameters);
            return response.Deserialize<T>();
        }

        public IObservable<T> StartObserving<T>(string request, int timeout)
        {
            return Observable.Create<T>((observer, cancellationToken) =>
            {
                return Task.Run(async () =>
                {
                    var etag = string.Empty;
                    while (true)
                    {
                        var parameters = HttpUtility.ParseQueryString(string.Empty);
                        parameters["timeout"] = timeout.ToString();
                        parameters["etag"] = etag.ToString();
                        try
                        {
                            var document = await SendRequest(request, parameters, cancellationToken);
                            var response = document.Deserialize<T>();

                            observer.OnNext(response);
                            etag = (string)document.Root.Attribute("etag");
                        }
                        catch(OperationCanceledException)
                        {
                            observer.OnCompleted();
                            break;
                        }
                    }

                }, cancellationToken);
            
            }).Publish().RefCount();
        }

        public async Task<Status> GetStatus()
        {
            return await SendRequest<Status>("Status");
        }

        public async Task<SyncStatus> GetSyncStatus()
        {
            return await SendRequest<SyncStatus>("SyncStatus");
        }
    }
}
