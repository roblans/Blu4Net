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
        public Volume Volume { get; private set; } 
        public IObservable<Volume> VolumeChanges { get; }

        private BluPlayer(BluChannel channel, SyncStatusResponse syncStatus, StatusResponse status)
        {
            _channel = channel ?? throw new ArgumentNullException(nameof(channel));
            _syncStatus = syncStatus ?? throw new ArgumentNullException(nameof(syncStatus));
            _status = status ?? throw new ArgumentNullException(nameof(status));

            Volume = Volume.FromPercentage(_status.Volume);
            VolumeChanges = channel.VolumeChanges.Select(response => Volume = Volume.FromResponse(response));
            VolumeChanges.Subscribe(value => Volume = value);
        }

        public async Task SetVolume(Volume value)
        {
            if (Volume == null)
                throw new ArgumentNullException(nameof(value));

            await _channel.SetVolume(value.Percentage);
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
