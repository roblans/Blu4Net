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
    // https://nadelectronics.com/wp-content/uploads/2019/09/Custom-Integration-API-v1.0.pdf
    public class BluChannel
    {
        static readonly TimeSpan InfiniteTimeout = TimeSpan.FromMilliseconds(System.Threading.Timeout.Infinite);
        public Uri Endpoint { get; }
        public TimeSpan Timeout { get; } = TimeSpan.FromSeconds(5);
        public IObservable<StatusResponse> StatusChanges { get; }
        public IObservable<SyncStatusResponse> SyncStatusChanges { get; }

        public BluChannel(Uri endpoint)
        {
            Endpoint = endpoint ?? throw new ArgumentNullException(nameof(endpoint));

            // recommended long polling interval for Status is 100 seconds
            StatusChanges = LongPolling<StatusResponse>("Status", 100);

            // recommended long polling interval for SyncStatus changes is 180 seconds
            SyncStatusChanges = LongPolling<SyncStatusResponse>("SyncStatus", 180);
        }

        private async Task<T> SendRequest<T>(string request, NameValueCollection parameters, TimeSpan timeout, CancellationToken cancellationToken)
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
                    var document = XDocument.Load(stream);
                    return document.Deserialize<T>();
                }
            }
        }

        private Task<T> SendRequest<T>(string request, NameValueCollection parameters = null)
        {
            return SendRequest<T>(request, parameters, Timeout, CancellationToken.None);
        }

        private IObservable<T> LongPolling<T>(string request, int timeout) where T : LongPollingResponse
        {
            return Observable.Create<T>((observer, cancellationToken) =>
            {
                return Task.Run(async () =>
                {
                    var longPollingTag = default(string);
                    while (true)
                    {
                        var parameters = HttpUtility.ParseQueryString(string.Empty);
                        if (longPollingTag != null)
                        {
                            parameters["timeout"] = timeout.ToString();
                            parameters["etag"] = longPollingTag.ToString();
                        }
                        try
                        {
                            var response = await SendRequest<T>(request, parameters, InfiniteTimeout, cancellationToken);
                            longPollingTag = response.ETag;
                            observer.OnNext(response);
                        }
                        catch(OperationCanceledException)
                        {
                            observer.OnCompleted();
                            break;
                        }
                    }

                }, cancellationToken);
            });
        }

        public async Task<StatusResponse> GetStatus()
        {
            return await SendRequest<StatusResponse>("Status");
        }

        public async Task<SyncStatusResponse> GetSyncStatus()
        {
            return await SendRequest<SyncStatusResponse>("SyncStatus");
        }
    }
}
