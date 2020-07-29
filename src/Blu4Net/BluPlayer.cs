using Blu4Net.Channel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Zeroconf;

namespace Blu4Net
{
    public class BluPlayer
    {
        private readonly BluChannel _channel;
        private readonly List<IDisposable> _subscriptions = new List<IDisposable>();
        private StatusResponse _statusResponse;

        public string Name { get; private set; }
        public Uri Endpoint { get; }
        
        public int Volume { get; private set; }
        public IObservable<int> VolumeChanges { get; private set; }

        public PlayerState State { get; private set; }
        public IObservable<PlayerState> StateChanges { get; private set; }

        public PlayerMode Mode { get; private set; }
        public IObservable<PlayerMode> ModeChanges { get; private set; }

        public PlayerMedia Media { get; private set; }
        public IObservable<PlayerMedia> MediaChanges { get; private set; }

        public IReadOnlyList<PlayerPreset> Presets { get; private set; }
        public IObservable<IReadOnlyList<PlayerPreset>> PresetsChanges { get; private set; }

        public BluPlayer(Uri endpoint)
        {
            Endpoint = endpoint ?? throw new ArgumentNullException(nameof(endpoint));
            _channel = new BluChannel(Endpoint);
        }

        public BluPlayer(IPAddress address, int port = 11000)
        {
            if (address == null)
                throw new ArgumentNullException(nameof(address));

            Endpoint = new UriBuilder("http", address.ToString(), port).Uri;
            _channel = new BluChannel(Endpoint);
        }

        public BluPlayer(string host, int port = 11000)
        {
            if (host == null)
                throw new ArgumentNullException(nameof(host));

            Endpoint = new UriBuilder("http", host, port).Uri;
            _channel = new BluChannel(Endpoint);
        }

        public async Task Connect()
        {
            var syncStatusResponse = await _channel.GetSyncStatus();
            var statusResponse = await _channel.GetStatus();
            var presetsResponse = await _channel.GetPresets();

            Name = syncStatusResponse.Name;

            var subscription = default(IDisposable);
            Volume = statusResponse.Volume;
            VolumeChanges = _channel.VolumeChanges
                .Select(response => response.Volume)
                .DistinctUntilChanged();
            subscription = VolumeChanges.Subscribe(volume => Volume = volume);
            _subscriptions.Add(subscription);

            State = ParseState(statusResponse.State);
            StateChanges = _channel.StatusChanges
                .Do(response => _statusResponse = response)
                .Select(response => ParseState(response.State))
                .DistinctUntilChanged();
            subscription = StateChanges.Subscribe(state => State = state);
            _subscriptions.Add(subscription);

            Mode = ParseMode(statusResponse.Shuffle, statusResponse.Repeat);
            ModeChanges = _channel.StatusChanges
                .Select(response => ParseMode(response.Shuffle, response.Repeat))
                .DistinctUntilChanged();
            subscription = ModeChanges.Subscribe(mode => Mode = mode);
            _subscriptions.Add(subscription);

            Media = ParseMedia(statusResponse);
            MediaChanges = _channel.StatusChanges
                .Select(response => ParseMedia(response))
                .DistinctUntilChanged();
            subscription = MediaChanges.Subscribe(media => Media = media);
            _subscriptions.Add(subscription);

            Presets = ParsePresets(presetsResponse);
            PresetsChanges = _channel.StatusChanges
                .Select(response => response.PresetID)
                .DistinctUntilChanged()
                .SelectAsync(async _ => await _channel.GetPresets())
                .Select(response => ParsePresets(response));

            subscription = PresetsChanges.Subscribe(presets => Presets = presets);
            _subscriptions.Add(subscription);
        }

        public ValueTask Disconnect()
        {
            foreach (var subscription in _subscriptions)
            {
                subscription.Dispose();
            }
            return default;
        }

        private IReadOnlyList<PlayerPreset> ParsePresets(PresetsResponse response)
        {
            return response.Presets
                .Select(element => new PlayerPreset(element.ID, element.Name, ParseUri(element.Image)))
                .ToArray();
        }

        private Uri ParseUri(string value)
        {
            if (value != null && Uri.TryCreate(value, UriKind.RelativeOrAbsolute, out var uri))
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
            if (response == null)
                throw new ArgumentNullException(nameof(response));

            var imageUri = response.Image != null ? ParseUri(response.Image) : null;
            var serviceIconUri = response.ServiceIcon != null ? ParseUri(response.ServiceIcon) : null;
            var titles = new[] { response.Title1, response.Title2, response.Title3 }.Where(element => element != null).ToArray();

            return new PlayerMedia(titles, imageUri, serviceIconUri);
        }

        private PlayerState ParseState(string value)
        {
            if (value != null)
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


        public TextWriter Log
        {
            get { return _channel.Log;  }
            set { _channel.Log = value; }
        }

        public async Task<int> SetVolume(int percentage)
        {
            if (percentage < 0 || percentage > 100)
                throw new ArgumentOutOfRangeException(nameof(percentage), "Value must be between 0 and 100");

            var response = await _channel.SetVolume(percentage);
            return Volume = response.Volume;
        }

        public async Task<int> Mute(bool on = true)
        {
            var response = await _channel.Mute(on ? 1 : 0);
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
            if (_statusResponse != null && _statusResponse.StreamUrl == null)
            { 
                var response = await _channel.Back();
                return response?.ID;
            }
            return null;
        }

        public async Task<int?> Skip()
        {
            if (_statusResponse != null && _statusResponse.StreamUrl == null)
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
    }
}
