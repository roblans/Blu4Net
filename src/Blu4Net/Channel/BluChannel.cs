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
        public TimeSpan Timeout { get; } = TimeSpan.FromSeconds(30);

        public BluChannel(Uri endpoint)
        {
            Endpoint = endpoint ?? throw new ArgumentNullException(nameof(endpoint));
        }

        private async Task<XDocument> SendRequest(string request, NameValueCollection parameters, TimeSpan timeout, CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var requestUri = new UriBuilder(Endpoint)
            {
                Path = request,
                Query = parameters != null && parameters.Count > 0 ? parameters.ToString() : null,
            }.Uri;

            using (var client = new HttpClient() { Timeout = timeout })
            {
                using (var response = await client.GetAsync(requestUri, cancellationToken))
                using (var stream = await response.Content.ReadAsStreamAsync())
                {
                    return XDocument.Load(stream);
                }
            }
        }

        private async Task<T> SendRequest<T>(string request, NameValueCollection parameters, TimeSpan timeout, CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var document = await SendRequest(request, parameters, timeout, cancellationToken);
            return document.Deserialize<T>();
        }

        private Task<T> SendRequest<T>(string request, NameValueCollection parameters = null)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return SendRequest<T>(request, parameters, Timeout, CancellationToken.None);
        }

        private IObservable<T> LongPolling<T>(string request, int timeout) where T : LongPollingResponse
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));
            if (timeout < 0)
                throw new ArgumentOutOfRangeException(nameof(timeout), "Value must be greater than zero");

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
                            if (!object.Equals(longPollingTag, response.ETag))
                            {
                                observer.OnNext(response);
                            }
                            longPollingTag = response.ETag;
                        }
                        catch (OperationCanceledException)
                        {
                            observer.OnCompleted();
                            break;
                        }
                    }

                }, cancellationToken);
            });
        }

        public IObservable<StatusResponse> StatusChanges()
        {
            // recommended long polling interval for Status is 100 seconds
            return LongPolling<StatusResponse>("Status", 100);
        }

        public IObservable<SyncStatusResponse> SyncStatusChanges()
        {
            // recommended long polling interval for SyncStatus changes is 180 seconds
            return LongPolling<SyncStatusResponse>("SyncStatus", 180);
        }

        public IObservable<VolumeResponse> VolumeChanges()
        {
            // recommended long polling interval for volume is not specified (use 100)
            return LongPolling<VolumeResponse>("Volume", 100);
        }

        public async Task<StatusResponse> GetStatus()
        {
            return await SendRequest<StatusResponse>("Status");
        }

        public async Task<SyncStatusResponse> GetSyncStatus()
        {
            return await SendRequest<SyncStatusResponse>("SyncStatus");
        }

        public Task<PlayResponse> Play()
        {
            return SendRequest<PlayResponse>("Play");
        }

        public Task<PlayResponse> Play(int seek)
        {
            if (seek < 0)
                throw new ArgumentException(nameof(seek), "Value must be greater than zero");

            var parameters = HttpUtility.ParseQueryString(string.Empty);
            parameters["seek"] = seek.ToString();
            return SendRequest<PlayResponse>("Play", parameters);
        }

        public async Task<PlayResponse> Pause(int toggle = 0)
        {
            if (toggle < 0 || toggle > 1)
                throw new ArgumentOutOfRangeException(nameof(toggle), "toggle must be 0 or 1");

            var parameters = HttpUtility.ParseQueryString(string.Empty);
            if (toggle == 1)
            {
                parameters["toggle"] = toggle.ToString();
            }

            return await SendRequest<PlayResponse>("Pause", parameters);
        }

        public async Task<StopResponse> Stop()
        {
            return await SendRequest<StopResponse>("Stop");
        }

        public async Task<SkipResponse> Skip()
        {
            return await SendRequest<SkipResponse>("Skip");
        }

        public async Task<BackResponse> Back()
        {
            return await SendRequest<BackResponse>("Back");
        }

        public async Task<VolumeResponse> GetVolume()
        {
            return await SendRequest<VolumeResponse>("Volume");
        }

        public async Task<VolumeResponse> SetVolume(int percentage)
        {
            if (percentage < 0 || percentage > 100)
                throw new ArgumentOutOfRangeException(nameof(percentage), "Value must be between 0 and 100");

            var parameters = HttpUtility.ParseQueryString(string.Empty);
            parameters["level"] = percentage.ToString();
            return await SendRequest<VolumeResponse>("Volume", parameters);
        }

        public async Task<VolumeResponse> Mute(int mute = 1)
        {
            if (mute < 0 || mute > 1)
                throw new ArgumentOutOfRangeException(nameof(mute), "Value must be 0 or 1");

            var parameters = HttpUtility.ParseQueryString(string.Empty);
            parameters["mute"] = mute.ToString();
            return await SendRequest<VolumeResponse>("Volume", parameters);
        }

        public async Task<PlaylistStatusResponse> GetPlaylistStatus()
        {
            var parameters = HttpUtility.ParseQueryString(string.Empty);
            parameters["length"] = 1.ToString();
            return await SendRequest<PlaylistStatusResponse>("Playlist", parameters);
        }

        private async Task<PlaylistListingResponse> GetPlaylistList(int startIndex, int length)
        {
            if (startIndex < 0)
                throw new ArgumentOutOfRangeException(nameof(startIndex), "Value must be greater than zero");
            if (length < 0)
                throw new ArgumentOutOfRangeException(nameof(length), "Value must be greater than zero");

            var parameters = HttpUtility.ParseQueryString(string.Empty);
            if (startIndex > 0 || length != int.MaxValue)
            {
                parameters["start"] = startIndex.ToString();
                parameters["end"] = (startIndex + length - 1).ToString();
            }
            
            var response = await SendRequest<PlaylistListingResponse>("Playlist", parameters);
            if (response.Tracks == null)
            {
                response.Tracks = new PlaylistTrack[0];
            }
            return response;
        }

        public async IAsyncEnumerable<PlaylistListingResponse> GetPlaylistList(int batchSize)
        {
            var startIndex = 0;

            while (true)
            {
                var listing = await GetPlaylistList(startIndex, batchSize);
                if (listing.Tracks.Length == 0)
                    break;

                yield return listing;
                startIndex += listing.Tracks.Length;
            }
        }

        public Task<PlaylistListingResponse> GetPlaylistList()
        {
            return GetPlaylistList(0, int.MaxValue);
        }

        public async Task<ClearResponse> Clear()
        {
            return await SendRequest<ClearResponse>("Clear");
        }

        public Task<ShuffleResponse> GetShuffle()
        {
            return SendRequest<ShuffleResponse>("Shuffle");
        }

        public Task<ShuffleResponse> SetShuffle(int state = 1)
        {
            if (state < 0 || state > 1)
                throw new ArgumentOutOfRangeException(nameof(state), "Value must be 0 or 1");

            var parameters = HttpUtility.ParseQueryString(string.Empty);
            parameters["state"] = state.ToString();
            return SendRequest<ShuffleResponse>("Shuffle", parameters);
        }

        public Task<RepeatResponse> GetRepeat()
        {
            return SendRequest<RepeatResponse>("Repeat");
        }

        public Task<RepeatResponse> SetRepeat(int state)
        {
            if (state < 0 || state > 2)
                throw new ArgumentOutOfRangeException(nameof(state), "Value must be 0, 1 or 2");

            var parameters = HttpUtility.ParseQueryString(string.Empty);
            parameters["state"] = state.ToString();
            return SendRequest<RepeatResponse>("Repeat", parameters);
        }

        public async Task<PresetsResponse> GetPresets()
        {
            var response = await SendRequest<PresetsResponse>("Presets");
            if (response.Presets == null)
            {
                response.Presets = new Preset[0];
            }
            return response;
        }
    }
}
