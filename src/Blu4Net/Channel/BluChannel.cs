﻿using System;
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
        public IObservable<StatusResponse> StatusChanges { get; }
        public IObservable<SyncStatusResponse> SyncStatusChanges { get; }
        public IObservable<VolumeResponse> VolumeChanges { get; }

        public BluChannel(Uri endpoint)
        {
            Endpoint = endpoint ?? throw new ArgumentNullException(nameof(endpoint));

            // recommended long polling interval for Status is 100 seconds
            StatusChanges = LongPolling<StatusResponse>("Status", 100);

            // recommended long polling interval for SyncStatus changes is 180 seconds
            SyncStatusChanges = LongPolling<SyncStatusResponse>("SyncStatus", 180);

            // recommended long polling interval for is not specified (use 100)
            VolumeChanges = LongPolling<VolumeResponse>("Volume", 100);
        }

        private async Task<T> SendRequest<T>(string request, NameValueCollection parameters, TimeSpan timeout, CancellationToken cancellationToken)
        {
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

        public async Task<StatusResponse> GetStatus()
        {
            return await SendRequest<StatusResponse>("Status");
        }

        public async Task<SyncStatusResponse> GetSyncStatus()
        {
            return await SendRequest<SyncStatusResponse>("SyncStatus");
        }

        public async Task<PlayResponse> Play(int? seek = null)
        {
            var parameters = HttpUtility.ParseQueryString(string.Empty);
            if (seek != null)
            {
                parameters["seek"] = seek.Value.ToString();
            }

            return await SendRequest<PlayResponse>("Play", parameters);
        }

        public async Task<PlayResponse> Pause(bool toggle = false)
        {
            var parameters = HttpUtility.ParseQueryString(string.Empty);
            if (toggle)
            {
                parameters["toggle"] = 1.ToString();
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
            var parameters = HttpUtility.ParseQueryString(string.Empty);
            parameters["level"] = percentage.ToString();
            return await SendRequest<VolumeResponse>("Volume", parameters);
        }

        public async Task<VolumeResponse> Mute(bool mute = true)
        {
            var parameters = HttpUtility.ParseQueryString(string.Empty);
            parameters["mute"] = mute ? 1.ToString() : 0.ToString();
            return await SendRequest<VolumeResponse>("Volume", parameters);
        }

        public async Task<PlayQueueStatusResponse> GetPlayQueueStatus()
        {
            var parameters = HttpUtility.ParseQueryString(string.Empty);
            parameters["length"] = 1.ToString();
            return await SendRequest<PlayQueueStatusResponse>("Playlist", parameters);
        }

        public async Task<PlayQueueListingResponse> GetPlayQueueListing(int startIndex, int length)
        {
            var parameters = HttpUtility.ParseQueryString(string.Empty);
            if (length != 0)
            {
                parameters["start"] = startIndex.ToString();
                parameters["end"] = (startIndex + length - 1).ToString();
            }
            
            var response = await SendRequest<PlayQueueListingResponse>("Playlist", parameters);
            if (response.Tracks == null)
            {
                response.Tracks = new PlayQueueTrack[0];
            }
            return response;
        }

        public Task<PlayQueueListingResponse> GetPlayQueueListing()
        {
            return GetPlayQueueListing(0, 0);
        }

        public async Task<ClearPlayQueueResponse> ClearPlayQueue()
        {
            return await SendRequest<ClearPlayQueueResponse>("Clear");
        }

        public Task<ShuffleResponse> GetShuffle()
        {
            return SendRequest<ShuffleResponse>("Shuffle");
        }

        public Task<ShuffleResponse> SetShuffle(bool enable = true)
        {
            var parameters = HttpUtility.ParseQueryString(string.Empty);
            parameters["state"] = enable ? 1.ToString() : 0.ToString();
            return SendRequest<ShuffleResponse>("Shuffle", parameters);
        }

        public Task<RepeatResponse> GetRepeat()
        {
            return SendRequest<RepeatResponse>("Repeat");
        }

        public Task<RepeatResponse> SetRepeat(int state)
        {
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
