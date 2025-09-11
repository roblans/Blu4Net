using Blu4Net.Channel;
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace Blu4Net
{
    public class BluPlayer
    {
        private readonly BluChannel _channel;

        public string Name { get; }
        public string Brand { get; }
        public string MacAddress { get; }
        public Uri Endpoint { get; }

        public PlayerPresetList PresetList { get; }
        public PlayQueue PlayQueue { get; }
        public MusicBrowser MusicBrowser { get; }

        public IObservable<Volume> VolumeChanges { get; }
        public IObservable<PlayerState> StateChanges { get; }
        public IObservable<ShuffleMode> ShuffleModeChanges { get; }
        public IObservable<RepeatMode> RepeatModeChanges { get; }
        public IObservable<PlayerMedia> MediaChanges { get; }
        public IObservable<PlayPosition> PositionChanges { get; }
        public IObservable<GroupingState> GroupingChanges { get; }

        private BluPlayer(BluChannel channel, SyncStatusResponse synStatus, StatusResponse status, BrowseContentResponse content)
        {
            _channel = channel ?? throw new ArgumentNullException(nameof(channel));

            Endpoint = _channel.Endpoint;
            Name = synStatus.Name;
            Brand = synStatus.Brand;
            MacAddress = synStatus.MAC;

            PresetList = new PlayerPresetList(_channel, status);
            PlayQueue = new PlayQueue(_channel, status);
            MusicBrowser = new MusicBrowser(_channel, content);

            VolumeChanges = _channel.VolumeChanges
                .SkipWhile(response => response.Decibel == status.Decibel)
                .DistinctUntilChanged(response => response.Decibel)
                .Select(response => new Volume(response));

            StateChanges = _channel.StatusChanges
                .SkipWhile(response => response.State == status.State)
                .DistinctUntilChanged(response => response.State)
                .Select(response => BluParser.ParseState(response.State));

            ShuffleModeChanges = _channel.StatusChanges
                .SkipWhile(response => response.Shuffle == status.Shuffle)
                .DistinctUntilChanged(response => response.Shuffle)
                .Select(response => (ShuffleMode)response.Shuffle);

            RepeatModeChanges = _channel.StatusChanges
                .SkipWhile(response => response.Repeat == status.Repeat)
                .DistinctUntilChanged(response => response.Repeat)
                .Select(response => (RepeatMode)response.Repeat);

            MediaChanges = _channel.StatusChanges
                .SkipWhile(response => response.Title1 == status.Title1 && response.Title2 == status.Title2 && response.Title3 == status.Title3 && response.Quality == status.Quality)
                .DistinctUntilChanged(response => $"{response.Title1}{response.Title2}{response.Title3}{response.Quality}")
                .Select(response => new PlayerMedia(response, Endpoint));

            PositionChanges = _channel.StatusChanges
                .SkipWhile(response => response.Seconds == status.Seconds && response.TotalLength == status.TotalLength)
                .DistinctUntilChanged(response => $"{response.Seconds}{response.TotalLength}")
                .Select(response => new PlayPosition(response));

            var initialGroupingKey = SyncStatusKey(synStatus);

            GroupingChanges = _channel.SyncStatusChanges
                .SkipWhile(response => SyncStatusKey(response) == initialGroupingKey)
                .DistinctUntilChanged(response => SyncStatusKey(response))
                .Select(response => new GroupingState(response));
        }

        public static async Task<BluPlayer> Connect(Uri endpoint, CultureInfo acceptLanguage = null)
        {
            if (endpoint == null)
                throw new ArgumentNullException(nameof(endpoint));

            if (acceptLanguage == null)
            {
                acceptLanguage = new CultureInfo("en-US");
            }

            var channel = new BluChannel(endpoint, acceptLanguage);
            var syncStatus = await channel.GetSyncStatus().ConfigureAwait(false);
            var status = await channel.GetStatus().ConfigureAwait(false);
            var content = await channel.BrowseContent().ConfigureAwait(false);

            return new BluPlayer(channel, syncStatus, status, content);
        }

        public static Task<BluPlayer> Connect(IPAddress address, int port = 11000, CultureInfo acceptLanguage = null)
        {
            if (address == null)
                throw new ArgumentNullException(nameof(address));

            var endpoint = new UriBuilder("http", address.ToString(), port).Uri;
            return Connect(endpoint, acceptLanguage);
        }

        public static Task<BluPlayer> Connect(string host, int port = 11000, CultureInfo acceptLanguage = null)
        {
            if (host == null)
                throw new ArgumentNullException(nameof(host));

            var endpoint = new UriBuilder("http", host, port).Uri;
            return Connect(endpoint, acceptLanguage);
        }

        public TextWriter Log
        {
            get { return _channel.Log; }
            set { _channel.Log = value; }
        }

        public async Task<Volume> GetVolume()
        {
            var response = await _channel.GetVolume().ConfigureAwait(false);
            return new Volume(response);
        }

        public async Task<int> SetVolume(int percentage)
        {
            if (percentage < 0 || percentage > 100)
                throw new ArgumentOutOfRangeException(nameof(percentage), "Value must be between 0 and 100");

            var response = await _channel.SetVolume(percentage).ConfigureAwait(false);
            return response.Volume;
        }

        public async Task<int> Mute(bool on = true)
        {
            var response = await _channel.Mute(on ? 1 : 0).ConfigureAwait(false);
            return response.Volume;
        }

        public async Task<PlayerState> GetState()
        {
            var response = await _channel.GetStatus().ConfigureAwait(false);
            return BluParser.ParseState(response.State);
        }

        public async Task<PlayerState> Play()
        {
            var response = await _channel.Play().ConfigureAwait(false);
            return BluParser.ParseState(response.State);
        }

        public async Task<PlayerState> Play(int id)
        {
            var response = await _channel.PlayByID(id).ConfigureAwait(false);
            return BluParser.ParseState(response.State);
        }

        public async Task<PlayerState> Seek(TimeSpan offset)
        {
            var response = await _channel.Play((int)offset.TotalSeconds).ConfigureAwait(false);
            return BluParser.ParseState(response.State);
        }

        public async Task<PlayerState> Pause(bool toggle = false)
        {
            var response = await _channel.Pause(toggle ? 1 : 0).ConfigureAwait(false);
            return BluParser.ParseState(response.State);
        }

        public async Task<PlayerState> Stop()
        {
            var response = await _channel.Stop().ConfigureAwait(false);
            return BluParser.ParseState(response.State);
        }

        public async Task<int?> Back()
        {
            var status = await _channel.GetStatus().ConfigureAwait(false);
            if (status.StreamUrl == null)
            {
                var response = await _channel.Back().ConfigureAwait(false);
                return response?.ID;
            }
            return null;
        }

        public async Task<int?> Skip()
        {
            var status = await _channel.GetStatus().ConfigureAwait(false);
            if (status.StreamUrl == null)
            {
                var response = await _channel.Skip().ConfigureAwait(false);
                return response?.ID;
            }
            return null;
        }

        public async Task<ShuffleMode> GetShuffleMode()
        {
            var response = await _channel.GetShuffle().ConfigureAwait(false);
            return (ShuffleMode)response.Shuffle;
        }

        public async Task<ShuffleMode> SetShuffleMode(ShuffleMode mode = ShuffleMode.ShuffleOn)
        {
            var response = await _channel.SetShuffle((int)mode).ConfigureAwait(false);
            return (ShuffleMode)response.Shuffle;
        }

        public async Task<RepeatMode> GetRepeatMode()
        {
            var response = await _channel.GetRepeat().ConfigureAwait(false);
            return (RepeatMode)response.Repeat;
        }

        public async Task<RepeatMode> SetRepeatMode(RepeatMode mode = RepeatMode.RepeatAll)
        {
            var response = await _channel.SetRepeat((int)mode).ConfigureAwait(false);
            return (RepeatMode)response.Repeat;
        }

        public async Task<PlayerMedia> GetMedia()
        {
            var response = await _channel.GetStatus().ConfigureAwait(false);
            return new PlayerMedia(response, Endpoint);
        }

        public async Task<PlayPosition> GetPosition()
        {
            var response = await _channel.GetStatus().ConfigureAwait(false);
            return new PlayPosition(response);
        }

        public async Task<AddSlaveResponse> AddSlave(BluPlayer slave)
        {
            return await AddSlave(slave, createStereoPair: false);
        }
        public async Task<AddSlaveResponse> AddSlave(BluPlayer slave, bool createStereoPair)
        {
            return await AddSlave(slave, createStereoPair: createStereoPair, ChannelMode.Right);
        }
        public async Task<AddSlaveResponse> AddSlave(BluPlayer slave, bool createStereoPair, ChannelMode slaveChannel, string groupName = null)
        {
            if (slave == null)
                throw new ArgumentNullException(nameof(slave));

            if (this == slave)
                throw new ArgumentException("Cannot add self as slave", nameof(slave));

            SyncStatusResponse syncStatus = await _channel.GetSyncStatus();
            if (syncStatus.Master != null)
                throw new ArgumentException("Cannot add slave to slave", nameof(slave));

            if (syncStatus.Slave?.Any(s => s.Address == slave.Endpoint.Host) == true)
                throw new ArgumentException($"Player '{slave.Name}' is already a slave", nameof(slave));


            return await _channel.AddSlave(slave.Endpoint.Host, slave.Endpoint.Port, createStereoPair, slaveChannel, groupName);
        }

        public async Task<GroupingState> GetGroupingState()
        {
            var response = await _channel.GetSyncStatus().ConfigureAwait(false);
            return new GroupingState(response);
        }

        public async Task<SyncStatusResponse> RemoveSlave(BluPlayer slave)
        {
            if (slave == null)
                throw new ArgumentNullException(nameof(slave));

            if (this == slave)
                throw new ArgumentException("Cannot remove self as slave", nameof(slave));

            SyncStatusResponse syncStatus = await _channel.GetSyncStatus();

            bool isSyncSlave = syncStatus.Slave?.Any(s => s.Address == slave.Endpoint.Host) == true;
            bool isFixedGroupSlave = syncStatus.ZoneSlave?.Address == slave.Endpoint.Host == true;

            if (isSyncSlave == false && isFixedGroupSlave == false)
                throw new ArgumentException($"Player '{slave.Name}' is not a slave", nameof(slave));

            return await _channel.RemoveSlave(slave.Endpoint.Host, slave.Endpoint.Port);
        }

        public async Task<SyncStatusResponse> ZoneUngroup()
        {
            SyncStatusResponse syncStatus = await _channel.GetSyncStatus();

            if (syncStatus.IsZoneController == false)
                throw new ArgumentException("Player is not a zone controller");

            await _channel.ActionURL(syncStatus.ZoneUngroupUrl);

            return await _channel.GetSyncStatus();
        }

        public async Task<string> Action(string actionUri)
        {
            var response = await _channel.ActionURL(actionUri).ConfigureAwait(false);
            if (response is NotificationActionResponse notification)
            {
                return notification.Text;
            }
            return null;
        }

        private static string SyncStatusKey(Channel.SyncStatusResponse r)
        {
            if (r == null) return string.Empty;
            var slaves = r.Slave?.Select(s => $"{s.Address}:{s.Port}").OrderBy(x => x) ?? Enumerable.Empty<string>();
            return string.Concat(
                r.IsZoneController ? "ZC" : "NZC", "|",
                string.Join(",", slaves), "|",
                r.ZoneSlave == null ? string.Empty : $"{r.ZoneSlave.Address}:{r.ZoneSlave.Port}", "|",
                r.Master == null ? string.Empty : $"{r.Master.Address}:{r.Master.Port}"
            );
        }

        public void UpdateAcceptLanguage(CultureInfo culture)
        {
            _channel.AcceptLanguage = culture;
        }

        public override string ToString()
        {
            return Name;
        }
    }


}
