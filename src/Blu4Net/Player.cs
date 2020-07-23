using Blu4Net.Channel;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Blu4Net
{
    public class Player
    {
        private BluChannel _channel;

        private Player(BluChannel channel)
        {
            _channel = channel ?? throw new ArgumentNullException(nameof(channel));
        }

        public static Player Connect(IPAddress address, int port = 11000)
        {
            var channel = new BluChannel(new UriBuilder("http", address.ToString(), port).Uri);
            return new Player(channel);
        }

        public async Task<Status> GetStatus()
        {
            return await _channel.GetStatus();
        }

        public async Task<SyncStatus> GetSyncStatus()
        {
            return await _channel.GetSyncStatus();
        }
    }
}
