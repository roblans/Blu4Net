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

        public string Name { get; }
        public string Brand { get; }
        public Uri Endpoint { get; }

        public PlayerPresetList PresetList { get; private set; }
        public PlayQueue PlayQueue { get; private set; }

        public IObservable<int> VolumeChanges { get;  }
        public IObservable<PlayerState> StateChanges { get;  }
        public IObservable<ShuffleMode> ShuffleModeChanges { get; }
        public IObservable<RepeatMode> RepeatModeChanges { get; }
        public IObservable<PlayerMedia> MediaChanges { get; }

        private BluPlayer(BluChannel channel, SyncStatusResponse synStatus, StatusResponse status)
        {
            _channel = channel ?? throw new ArgumentNullException(nameof(channel));

            Endpoint = _channel.Endpoint;
            Name = synStatus.Name;
            Brand = synStatus.Brand;

            PresetList = new PlayerPresetList(_channel, status);
            PlayQueue = new PlayQueue(_channel, status);

            VolumeChanges = _channel.VolumeChanges
                .SkipWhile(response => response.Volume == status.Volume)
                .DistinctUntilChanged(response => response.Volume)
                .Select(response => response.Volume);

            StateChanges = _channel.StatusChanges
                .SkipWhile(response => response.State == status.State)
                .DistinctUntilChanged(response => response.State)
                .Select(response => ParseState(response.State));

            ShuffleModeChanges = _channel.StatusChanges
                .SkipWhile(response => response.Shuffle == status.Shuffle)
                .DistinctUntilChanged(response => response.Shuffle)
                .Select(response => (ShuffleMode)response.Shuffle);

            RepeatModeChanges = _channel.StatusChanges
                .SkipWhile(response => response.Repeat == status.Repeat)
                .DistinctUntilChanged(response => response.Repeat)
                .Select(response => (RepeatMode)response.Repeat);

            MediaChanges = _channel.StatusChanges
                .SkipWhile(response => response.Title1 == status.Title1 && response.Title2 == status.Title2 && response.Title3 == status.Title3)
                .DistinctUntilChanged(response => $"{response.Title1}{response.Title2}{response.Title3}")
                .Select(response => ParseMedia(response));
        }

        public static async Task<BluPlayer> Connect(Uri endpoint)
        {
            if (endpoint == null)
                throw new ArgumentNullException(nameof(endpoint));
            
            var channel = new BluChannel(endpoint);
            var syncStatus = await channel.GetSyncStatus();
            var status = await channel.GetStatus();

            return new BluPlayer(channel, syncStatus, status);
        }

        public static Task<BluPlayer> Connect(IPAddress address, int port = 11000)
        {
            if (address == null)
                throw new ArgumentNullException(nameof(address));

            var endpoint = new UriBuilder("http", address.ToString(), port).Uri;
            return Connect(endpoint);
        }

        public static Task<BluPlayer> Connect(string host, int port = 11000)
        {
            if (host == null)
                throw new ArgumentNullException(nameof(host));

            var endpoint = new UriBuilder("http", host, port).Uri;
            return Connect(endpoint);
        }

        private PlayerMedia ParseMedia(StatusResponse response)
        {
            if (response == null)
                throw new ArgumentNullException(nameof(response));

            var imageUri = response.Image != null ? response.Image.ToAbsoluteUri(Endpoint) : null;
            var serviceIconUri = response.ServiceIcon != null ? response.ServiceIcon.ToAbsoluteUri(Endpoint): null;
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

        public TextWriter Log
        {
            get { return _channel.Log;  }
            set { _channel.Log = value; }
        }

        public async Task<int> GetVolume()
        {
            var response = await _channel.GetVolume();
            return response.Volume;
        }

        public async Task<int> SetVolume(int percentage)
        {
            if (percentage < 0 || percentage > 100)
                throw new ArgumentOutOfRangeException(nameof(percentage), "Value must be between 0 and 100");

            var response = await _channel.SetVolume(percentage);
            return response.Volume;
        }

        public async Task<int> Mute(bool on = true)
        {
            var response = await _channel.Mute(on ? 1 : 0);
            return response.Volume;
        }

        public async Task<PlayerState> GetState()
        {
            var response = await _channel.GetStatus();
            return ParseState(response.State);
        }

        public async Task<PlayerState> Play()
        {
            var response = await _channel.Play();
            return ParseState(response.State);
        }

        public async Task<PlayerState> Pause(bool toggle = false)
        {
            var response = await _channel.Pause(toggle ? 1 : 0);
            return ParseState(response.State);
        }

        public async Task<PlayerState> Stop()
        {
            var response = await _channel.Stop();
            return ParseState(response.State);
        }

        public async Task<int?> Back()
        {
            var status = await _channel.GetStatus();
            if (status.StreamUrl == null)
            { 
                var response = await _channel.Back();
                return response?.ID;
            }
            return null;
        }

        public async Task<int?> Skip()
        {
            var status = await _channel.GetStatus();
            if (status.StreamUrl == null)
            {
                var response = await _channel.Skip();
                return response?.ID;
            }
            return null;
        }

        public async Task<ShuffleMode> GetShuffleMode()
        {
            var response = await _channel.GetShuffle();
            return (ShuffleMode)response.Shuffle;
        }

        public async Task<ShuffleMode> SetShuffleMode(ShuffleMode mode = ShuffleMode.ShuffleOn)
        {
            var response = await _channel.SetShuffle((int)mode);
            return (ShuffleMode)response.Shuffle;
        }

        public async Task<RepeatMode> GetRepeatMode()
        {
            var response = await _channel.GetRepeat();
            return (RepeatMode)response.Repeat;
        }

        public async Task<RepeatMode> SetRepeatMode(RepeatMode mode = RepeatMode.RepeatAll)
        {
            var response = await _channel.SetRepeat((int)mode);
            return (RepeatMode)response.Repeat;
        }

        public async Task<PlayerMedia> GetMedia()
        {
            var response = await _channel.GetStatus();
            return ParseMedia(response);
        }

        public async Task<IReadOnlyCollection<PlayerMusicSource>> GetMusicSources()
        {
            var response = await _channel.BrowseContent();

            return response.Items
                .Select(element => new PlayerMusicSource(_channel, element.BrowseKey, element.Text))
                .ToArray();
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
