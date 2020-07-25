using Blu4Net.Channel;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Zeroconf;

namespace Blu4Net
{
    public class BluPlayer
    {
        private BluChannel _channel;
        public string Name { get; }
        public Uri Endpoint => _channel.Endpoint;

        private BluPlayer(BluChannel channel, string name)
        {
            _channel = channel ?? throw new ArgumentNullException(nameof(channel));
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public static async Task<BluPlayer> Connect(Uri endpoint)
        {
            var channel = new BluChannel(endpoint);
            var status = await channel.GetSyncStatus();
            return new BluPlayer(channel, status.Name);
        }

        public static Task<BluPlayer> Connect(IPAddress address, int port = 11000)
        {
            return Connect(new UriBuilder("http", address.ToString(), port).Uri);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
