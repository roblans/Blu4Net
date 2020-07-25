using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Zeroconf;

namespace Blu4Net
{
    public class BluEnvironment
    {
        public static async IAsyncEnumerable<Uri> FindPlayerEndpoints()
        {
            var hosts = await ZeroconfResolver.ResolveAsync("_musc._tcp.local.");
            foreach (var host in hosts)
            {
                if (host.IPAddress == null)
                    continue;

                var address = IPAddress.Parse(host.IPAddress);
                var port = 11000;

                if (host.Services.TryGetValue("_musc._tcp.local.", out var service))
                {
                    port = service.Port;
                }

                yield return new UriBuilder("http", address.ToString(), port).Uri;
            }
        }

        public static async IAsyncEnumerable<BluPlayer> FindPlayers()
        {
            await foreach(var endpoint in FindPlayerEndpoints())
            {
                yield return await BluPlayer.Connect(endpoint);
            }
        }
    }
}
