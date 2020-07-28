using Blu4Net.Channel;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using Zeroconf;

namespace Blu4Net
{
    public class BluPlayer : IDisposable
    {
        private readonly BluChannel _channel;
        private readonly List<IDisposable> _subscriptions = new List<IDisposable>();

        private StatusResponse _status;
        private SyncStatusResponse _syncStatus;

        public string Name => _syncStatus.Name;
        public Uri Endpoint => _channel.Endpoint;
        
        public int Volume { get; private set; }
        public IObservable<int> VolumeChanges { get; }

        public PlayerState State { get; private set; }
        public IObservable<PlayerState> StateChanges { get; }

        public PlayerMode Mode { get; private set; }
        public IObservable<PlayerMode> ModeChanges { get; }

        public PlayerMedia Media { get; private set; }
        public IObservable<PlayerMedia> MediaChanges { get; }

        private BluPlayer(BluChannel channel, SyncStatusResponse syncStatus, StatusResponse status)
        {
            _channel = channel ?? throw new ArgumentNullException(nameof(channel));
            _syncStatus = syncStatus ?? throw new ArgumentNullException(nameof(syncStatus));
            _status = status ?? throw new ArgumentNullException(nameof(status));

            var subscription = default(IDisposable);
            Volume = status.Volume;
            VolumeChanges = channel.VolumeChanges
                .Select(response => response.Volume)
                .DistinctUntilChanged();
            subscription = VolumeChanges.Subscribe(volume => Volume = volume);
            _subscriptions.Add(subscription);

            State = ParseState(status.State);
            StateChanges = channel.StatusChanges
                .Do(status => _status = status)
                .Select(response => ParseState(response.State))
                .DistinctUntilChanged();
            subscription = StateChanges.Subscribe(state => State = state);
            _subscriptions.Add(subscription);

            Mode = ParseMode(status.Shuffle, status.Repeat);
            ModeChanges = channel.StatusChanges
                .Select(response => ParseMode(response.Shuffle, response.Repeat))
                .DistinctUntilChanged();
            subscription = ModeChanges.Subscribe(mode => Mode = mode);
            _subscriptions.Add(subscription);

            Media = ParseMedia(status);
            MediaChanges = channel.StatusChanges
                .Select(response => ParseMedia(response))
                .DistinctUntilChanged();
            subscription = ModeChanges.Subscribe(mode => Mode = mode);
            _subscriptions.Add(subscription);
        }

        private Uri ParseUri(string value)
        {
            if (Uri.TryCreate(value, UriKind.RelativeOrAbsolute, out var uri))
            {
                if (uri.IsAbsoluteUri)
                {
                    return uri;
                }
                return new Uri(Endpoint, uri);
            }
            return null;
        }

        private PlayerMedia ParseMedia(StatusResponse response)
        {
            var imageUri = response.Image != null ? ParseUri(response.Image) : null;
            var serviceIconUri = response.ServiceIcon != null ? ParseUri(response.ServiceIcon) : null;

            return new PlayerMedia(new[] { response.Title1, response.Title2, response.Title3 }, imageUri, serviceIconUri);
        }

        private PlayerState ParseState(string value)
        {
            switch (value)
            {
                case "stream":
                    return PlayerState.Streaming;
                case "play":
                    return PlayerState.Playing;
                case "pause":
                    return PlayerState.Paused;
                case "stop":
                    return PlayerState.Stopped;
                case "connecting":
                    return PlayerState.Connecting;
            }

            return PlayerState.Unknown;
        }

        private PlayerMode ParseMode(int shuffle, int repeat)
        {
            var shuffleMode = ShuffleMode.ShuffleOff;
            switch(shuffle)
            {
                case 0:
                    shuffleMode = ShuffleMode.ShuffleOff;
                    break;
                case 1:
                    shuffleMode = ShuffleMode.ShuffleOn;
                    break;
            }

            var repeatMode = RepeatMode.RepeatOff;
            switch (repeat)
            {
                case 0:
                    repeatMode = RepeatMode.RepeatAll;
                    break;
                case 1:
                    repeatMode = RepeatMode.RepeatOne;
                    break;
                case 2:
                    repeatMode = RepeatMode.RepeatOff;
                    break;
            }

            return new PlayerMode(shuffleMode, repeatMode);
        }

        public static async Task<BluPlayer> Connect(Uri endpoint)
        {
            var channel = new BluChannel(endpoint);
            var syncStatus = await channel.GetSyncStatus();
            var status = await channel.GetStatus();
            return new BluPlayer(channel, syncStatus, status);
        }

        public static Task<BluPlayer> Connect(IPAddress address, int port = 11000)
        {
            return Connect(new UriBuilder("http", address.ToString(), port).Uri);
        }

        public async Task<int> SetVolume(int value)
        {
            var response = await _channel.SetVolume(value);
            return Volume = response.Volume;
        }

        public async Task<PlayerState> Play()
        {
            var response = await _channel.Play();
            return State = ParseState(response.State);
        }

        public async Task<PlayerState> Pause(bool toggle = false)
        {
            var response = await _channel.Pause(toggle ? 1 : 0);
            return State = ParseState(response.State);
        }

        public async Task<PlayerState> Stop()
        {
            var response = await _channel.Stop();
            return State = ParseState(response.State);
        }

        public async Task<int?> Back()
        {
            if (_status.StreamUrl == null)
            {
                var response = await _channel.Back();
                return response?.ID;
            }
            return null;
        }

        public async Task<int?> Skip()
        {
            if (_status.StreamUrl == null)
            {
                var response = await _channel.Skip();
                return response?.ID;
            }
            return null;
        }

        public async Task<PlayerMode> SetMode(PlayerMode mode)
        {
            var shuffle = 0;
            switch (mode.Shuffle)
            {
                case ShuffleMode.ShuffleOff:
                    shuffle = 0;
                    break;
                case ShuffleMode.ShuffleOn:
                    shuffle = 1;
                    break;
            }
            var shuffleResponse = await _channel.SetShuffle(shuffle);

            var repeat = 0;
            switch (mode.Repeat)
            {
                case RepeatMode.RepeatAll:
                    repeat = 0;
                    break;
                case RepeatMode.RepeatOne:
                    repeat = 1;
                    break;
                case RepeatMode.RepeatOff:
                    repeat = 2;
                    break;
            }
            var repeatResponse = await _channel.SetRepeat(repeat);

            return ParseMode(shuffleResponse.Shuffle, repeatResponse.Repeat);
        }

        public override string ToString()
        {
            return Name;
        }

        public void Dispose()
        {
            foreach(var subscription in _subscriptions)
            {
                subscription.Dispose();
            }
        }
    }
}
