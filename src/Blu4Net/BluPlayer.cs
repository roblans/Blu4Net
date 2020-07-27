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
        public IObservable<int> VolumeChanges { get; }
        public int Volume { get; private set; }

        public IObservable<PlayerState> StateChanges { get; }
        public PlayerState State { get; private set; }

        private BluPlayer(BluChannel channel, SyncStatusResponse syncStatus, StatusResponse status)
        {
            _channel = channel ?? throw new ArgumentNullException(nameof(channel));
            _syncStatus = syncStatus ?? throw new ArgumentNullException(nameof(syncStatus));
            _status = status ?? throw new ArgumentNullException(nameof(status));

            var subscription = default(IDisposable);
            Volume = status.Volume;
            VolumeChanges = channel.VolumeChanges.Select(response => response.Volume);
            subscription = VolumeChanges.Subscribe(volume => Volume = volume);
            _subscriptions.Add(subscription);

            State = ParseState(status.State);
            StateChanges = channel.StatusChanges.Select(response => ParseState(response.State));
            subscription = StateChanges.Subscribe(state => State = state);
            _subscriptions.Add(subscription);
        }

        private static PlayerState ParseState(string value)
        {
            switch (value)
            {
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
            var response = await _channel.Pause(toggle ? 0 : 1);
            return State = ParseState(response.State);
        }

        public async Task<PlayerState> Stop()
        {
            var response = await _channel.Stop();
            return State = ParseState(response.State);
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
