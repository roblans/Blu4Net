using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Zeroconf;

namespace Blu4Net
{
    public class BluEnvironment
    {
        public static IObservable<Uri> PlayerEndpoints => ZeroconfResolver.Resolve("_musc._tcp.local.", TimeSpan.FromSeconds(30), 3, 2000).Select(host => GetEndpoint(host));
        public static IObservable<BluPlayer> Players => PlayerEndpoints.SelectAsync(endpoint => BluPlayer.Connect(endpoint));

        private static Uri GetEndpoint(IZeroconfHost host)
        {
            var address = IPAddress.Parse(host.IPAddress);
            var port = 11000;

            if (host.Services.TryGetValue("_musc._tcp.local.", out var service))
            {
                port = service.Port;
            }

            return new UriBuilder("http", address.ToString(), port).Uri;
        }
    }
}
