﻿using System;
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
        const int DefaultEndpointPort = 11000;

        private static Uri GetEndpoint(IZeroconfHost host)
        {
            var address = IPAddress.Parse(host.IPAddress);
            var port = DefaultEndpointPort;

            if (host.Services.TryGetValue("_musc._tcp.local.", out var service))
            {
                port = service.Port;
            }

            return new UriBuilder("http", address.ToString(), port).Uri;
        }

        public static IObservable<Uri> ResolveEndpoints(TimeSpan scanTime)
        {
            return ZeroconfResolver
              .Resolve("_musc._tcp.local.", scanTime)
              .Where(element => element.IPAddress != null)
              .Select(host => GetEndpoint(host));
        }

        public static IObservable<Uri> ResolveEndpoints()
        {
            return ResolveEndpoints(TimeSpan.FromSeconds(5));
        }

        public static IObservable<BluPlayer> ResolvePlayers(TimeSpan scanTime)
        {
            return  ResolveEndpoints(scanTime)
            .SelectAsync(endpoint => BluPlayer.Connect(endpoint));
        }

        public static IObservable<BluPlayer> ResolvePlayers()
        {
            return ResolvePlayers(TimeSpan.FromSeconds(5));
        }
    }
}
