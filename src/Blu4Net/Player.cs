using Blu4Net.Channel;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Zeroconf;

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

        public static async Task<Player[]> DiscoverPlayers()
        {
            var players = new List<Player>();

            var hosts = await ZeroconfResolver.ResolveAsync("_musc._tcp.local.");
            foreach (var host in hosts)
            {
                var address = IPAddress.Parse(host.IPAddress);
                var port = 11000;
                
                if (host.Services.TryGetValue("_musc._tcp.local.", out var service))
                {
                    port = service.Port;
                }

                players.Add(Connect(address, port));
            }

            return players.ToArray();
        }
    }
}
