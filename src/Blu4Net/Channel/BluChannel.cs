using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Linq;

namespace Blu4Net.Channel
{
    // https://nadelectronics.com/wp-content/uploads/2019/09/Custom-Integration-API-v1.0.pdf
    public class BluChannel
    {
#if FILE_LOGGING
        int counter = 0;
#endif
        public Uri Endpoint { get; }
        public TimeSpan Timeout { get; } = TimeSpan.FromSeconds(30);
        public TimeSpan RetryDelay { get; } = TimeSpan.FromSeconds(5);
        public CultureInfo AcceptLanguage { get; set; }
        public TextWriter Log { get; set; }
        public IObservable<StatusResponse> StatusChanges { get; }
        public IObservable<SyncStatusResponse> SyncStatusChanges { get; }
        public IObservable<VolumeResponse> VolumeChanges { get; }

        public BluChannel(Uri endpoint, CultureInfo acceptLanguage)
        {
            Endpoint = endpoint ?? throw new ArgumentNullException(nameof(endpoint));

            AcceptLanguage = acceptLanguage ?? throw new ArgumentNullException(nameof(acceptLanguage));

            // recommended long polling interval for Status is 100 seconds
            StatusChanges = LongPolling<StatusResponse>("Status", 100).Retry(RetryDelay).Publish().RefCount();

            // recommended long polling interval for SyncStatus changes is 180 seconds
            SyncStatusChanges = LongPolling<SyncStatusResponse>("SyncStatus", 180).Retry(RetryDelay).Publish().RefCount();

            // recommended long polling interval for volume is not specified (use 100)
            VolumeChanges = LongPolling<VolumeResponse>("Volume", 100).Retry(RetryDelay).Publish().RefCount();
        }

        private void LogMessage(string message)
        {
            if (Log != null && message != null)
            {
                Log.WriteLine(message);
            }
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

            LogMessage($"Request: {requestUri}");

            using (var client = new HttpClient() { Timeout = timeout })
            {
                client.DefaultRequestHeaders.AcceptLanguage.TryParseAdd(AcceptLanguage.Name);

                using (var response = await client.GetAsync(requestUri, cancellationToken).ConfigureAwait(false))
                using (var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                {
                    var document = XDocument.Load(stream);

                    LogMessage($"Response: {document}");

#if FILE_LOGGING
                    document.Save(@$"D:\Temp\{Interlocked.Increment(ref counter)}.txt");
#endif

                    return document;
                }
            }
        }

        private Task<XDocument> SendRequest(string request, NameValueCollection parameters = null)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return SendRequest(request, parameters, Timeout, CancellationToken.None);
        }

        private async Task<T> SendRequest<T>(string request, NameValueCollection parameters, TimeSpan timeout, CancellationToken cancellationToken)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            var document = await SendRequest(request, parameters, timeout, cancellationToken).ConfigureAwait(false);
            return document.Deserialize<T>();
        }

        private Task<T> SendRequest<T>(string request, NameValueCollection parameters = null)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            return SendRequest<T>(request, parameters, Timeout, CancellationToken.None);
        }

        private IObservable<T> LongPolling<T>(string request, int timeout) where T : ILongPollingResponse
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
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        var parameters = HttpUtility.ParseQueryString(string.Empty);
                        if (longPollingTag != null)
                        {
                            parameters["timeout"] = timeout.ToString();
                            parameters["etag"] = longPollingTag.ToString();
                        }
                        try
                        {
                            var response = await SendRequest<T>(request, parameters, TimeSpan.FromSeconds(1.5 * timeout), cancellationToken).ConfigureAwait(false);

                            if (longPollingTag != null)
                            {
                                observer.OnNext(response);
                            }
                            longPollingTag = response.ETag;
                        }
                        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                        {
                            observer.OnCompleted();
                            break;
                        }
                        catch (Exception error)
                        {
                            observer.OnError(error);
                            break;
                        }
                    }

                }, cancellationToken);
            });
        }


        public async Task<StatusResponse> GetStatus()
        {
            return await SendRequest<StatusResponse>("Status").ConfigureAwait(false);
        }

        public async Task<SyncStatusResponse> GetSyncStatus()
        {
            return await SendRequest<SyncStatusResponse>("SyncStatus").ConfigureAwait(false);
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

        public Task<AddSlaveResponse> AddSlave(string address, int port, bool createStereoPair, ChannelMode slaveChannel, string groupName)
        {
            if (address == null)
                throw new ArgumentNullException(nameof(address));
            if (address.Length == 0)
                throw new ArgumentOutOfRangeException(nameof(address), "Value cannot be an empty string");
            if (port < 1 || port > 65535)
                throw new ArgumentOutOfRangeException(nameof(port), "Value must be between 1 and 65535");

            var parameters = HttpUtility.ParseQueryString(string.Empty);
            parameters["slave"] = address;
            parameters["port"] = port.ToString();

            if (createStereoPair)
            {
                parameters["channelMode"] = slaveChannel == ChannelMode.Left ? ChannelMode.Right.ToString().ToLower() : ChannelMode.Left.ToString().ToLower();
                parameters["slaveChannelMode"] = slaveChannel.ToString().ToLower();
                if (string.IsNullOrWhiteSpace(groupName) == false)
                    parameters["group"] = groupName;
            }


            return SendRequest<AddSlaveResponse>("AddSlave", parameters);
        }

        public Task<SyncStatusResponse> RemoveSlave(string address, int port)
        {
            if (address == null)
                throw new ArgumentNullException(nameof(address));
            if (address.Length == 0)
                throw new ArgumentOutOfRangeException(nameof(address), "Value cannot be an empty string");
            if (port < 1 || port > 65535)
                throw new ArgumentOutOfRangeException(nameof(port), "Value must be between 1 and 65535");

            var parameters = HttpUtility.ParseQueryString(string.Empty);
            parameters["slave"] = address;
            parameters["port"] = port.ToString();

            return SendRequest<SyncStatusResponse>("RemoveSlave", parameters);
        }

        public Task<SyncStatusResponse> ZoneUngroup(string zoneUngroupUrl)
        {
            return SendRequest<SyncStatusResponse>(zoneUngroupUrl);
        }

        public Task<PlayResponse> PlayByID(int id)
        {
            if (id < 0)
                throw new ArgumentException(nameof(id), "Value must be greater than zero");

            var parameters = HttpUtility.ParseQueryString(string.Empty);
            parameters["id"] = id.ToString();
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

            return await SendRequest<PlayResponse>("Pause", parameters).ConfigureAwait(false);
        }

        public async Task<StopResponse> Stop()
        {
            return await SendRequest<StopResponse>("Stop").ConfigureAwait(false);
        }

        public async Task<SkipResponse> Skip()
        {
            return await SendRequest<SkipResponse>("Skip").ConfigureAwait(false);
        }

        public async Task<BackResponse> Back()
        {
            return await SendRequest<BackResponse>("Back").ConfigureAwait(false);
        }

        public async Task<VolumeResponse> GetVolume()
        {
            return await SendRequest<VolumeResponse>("Volume").ConfigureAwait(false);
        }

        public async Task<VolumeResponse> SetVolume(int percentage)
        {
            if (percentage < 0 || percentage > 100)
                throw new ArgumentOutOfRangeException(nameof(percentage), "Value must be between 0 and 100");

            var parameters = HttpUtility.ParseQueryString(string.Empty);
            parameters["level"] = percentage.ToString();
            return await SendRequest<VolumeResponse>("Volume", parameters).ConfigureAwait(false);
        }

        public async Task<VolumeResponse> Mute(int mute = 1)
        {
            if (mute < 0 || mute > 1)
                throw new ArgumentOutOfRangeException(nameof(mute), "Value must be 0 or 1");

            var parameters = HttpUtility.ParseQueryString(string.Empty);
            parameters["mute"] = mute.ToString();
            return await SendRequest<VolumeResponse>("Volume", parameters).ConfigureAwait(false);
        }

        public async Task<PlaylistStatusResponse> GetPlaylistStatus()
        {
            var parameters = HttpUtility.ParseQueryString(string.Empty);
            parameters["length"] = 1.ToString();
            return await SendRequest<PlaylistStatusResponse>("Playlist", parameters).ConfigureAwait(false);
        }

        private async Task<PlaylistResponse> GetPlaylist(int startIndex, int length)
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

            var response = await SendRequest<PlaylistResponse>("Playlist", parameters).ConfigureAwait(false);
            if (response.Songs == null)
            {
                response.Songs = new PlaylistResponse.Song[0];
            }
            return response;
        }

        public async IAsyncEnumerable<PlaylistResponse> GetPlaylistPaged(int pageSize)
        {
            var startIndex = 0;

            while (true)
            {
                var listing = await GetPlaylist(startIndex, pageSize).ConfigureAwait(false);
                if (listing.Songs.Length == 0)
                    break;

                yield return listing;
                startIndex += listing.Songs.Length;
            }
        }

        public Task<PlaylistResponse> GetPlaylist()
        {
            return GetPlaylist(0, int.MaxValue);
        }

        public async Task<ClearResponse> Clear()
        {
            return await SendRequest<ClearResponse>("Clear").ConfigureAwait(false);
        }

        public Task<DeleteResponse> Delete(int id)
        {
            var parameters = HttpUtility.ParseQueryString(string.Empty);
            parameters["id"] = id.ToString();
            return SendRequest<DeleteResponse>("Delete", parameters);
        }

        public Task<SavedResponse> Save(string name)
        {
            if (name == null)
                throw new ArgumentNullException(nameof(name));
            if (name.Length == 0)
                throw new ArgumentOutOfRangeException(nameof(name), "Value cannot be an empty string");

            var parameters = HttpUtility.ParseQueryString(string.Empty);
            parameters["name"] = name.ToString();
            return SendRequest<SavedResponse>("Save", parameters);
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
            var response = await SendRequest<PresetsResponse>("Presets").ConfigureAwait(false);
            if (response.Presets == null)
            {
                response.Presets = new PresetsResponse.Preset[0];
            }
            return response;
        }

        public async Task<LoadedResponse> LoadPreset(int id)
        {
            var parameters = HttpUtility.ParseQueryString(string.Empty);
            parameters["id"] = id.ToString();

            var document = await SendRequest("Preset", parameters).ConfigureAwait(false);
            if (document.Root.Name == "loaded")
            {
                return document.Deserialize<PlaylistLoadedResponse>();
            }
            if (document.Root.Name == "state")
            {
                return document.Deserialize<StreamLoadedResponse>();
            }
            throw new InvalidDataException();
        }

        public async Task<LoadedResponse> PlayURL(string playURL)
        {
            var parts = playURL.Split(new char[] { '?' });
            var parameters = HttpUtility.ParseQueryString(parts[1]);

            var document = await SendRequest(parts[0], parameters).ConfigureAwait(false);
            if (document.Root.Name == "loaded")
            {
                return document.Deserialize<PlaylistLoadedResponse>();
            }
            else if (document.Root.Name == "state")
            {
                return document.Deserialize<StreamLoadedResponse>();
            }
            else if (document.Root.Name == "addsong")
            {
                return document.Deserialize<AddSongResponse>();
            }
            throw new InvalidDataException();
        }

        public async Task<ActionResponse> ActionURL(string actionURL)
        {
            var parts = actionURL.Split(new char[] { '?' });
            var parameters = HttpUtility.ParseQueryString(parts[1]);

            var document = await SendRequest(parts[0], parameters).ConfigureAwait(false);
            if (document.Root.Name == "response")
            {
                return document.Deserialize<NotificationActionResponse>();
            }
            else if (document.Root.Name == "back")
            {
                return document.Deserialize<BackActionResponse>();
            }
            else if (document.Root.Name == "skip")
            {
                return document.Deserialize<SkipActionResponse>();
            }
            else if (document.Root.Name == "love")
            {
                return document.Deserialize<LoveActionResponse>();
            }
            else if (document.Root.Name == "ban")
            {
                return document.Deserialize<BanActionResponse>();
            }
            else if (document.Root.Name == "state")
            {
                return document.Deserialize<StateActionResponse>();
            }
            throw new InvalidDataException();
        }

        public async Task<BrowseContentResponse> BrowseContent(string key = null, string query = null)
        {
            var parameters = HttpUtility.ParseQueryString(string.Empty);
            if (key != null)
            {
                parameters["key"] = key;

                if (query != null)
                {
                    parameters["q"] = query;
                }
            }

            var response = await SendRequest<BrowseContentResponse>("Browse", parameters).ConfigureAwait(false);
            if (response.Items == null)
            {
                response.Items = new BrowseContentResponse.Item[0];
            }
            else
            {
                // note: TuneIn returns an empty <item></item> element  
                response.Items = response.Items.Where(element => element.Text != null).ToArray();
            }
            return response;
        }

        public Task<XDocument> GetServices()
        {
            return SendRequest("Services");
        }
    }
}
