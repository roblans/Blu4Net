![](https://dev.azure.com/roblans/Blu4Net/_apis/build/status/Blu4Net)

# Blu4Net
Blu4Net is a .NET library that interfaces with BluOS players (Bluesound, NAD). It uses an event-driven (RX), non-blocking model (Async/Await) that makes it lightweight and efficient.

Supported Targets:

- .NET Standard: 2.0
- .NET Standard: 2.1

## Features

### Discovery

Player endpoint discovery (mDNS)

### Basic operations

- Play
- Pause
- Stop
- Skip
- Back
- Shuffle (on | off)
- Repeat (off | one | all)
- Volume (percentage)
- Position (elapsed time + length of current track) 

### Media information

- Titles
- Image (artwork)
- Service icon

### Presets

- Fetch presets
- Load preset (number)

### Queue

- Info (name + length)
- Fetch songs (paged)

### Music sources (Library, TuneIn, ...)

- Fetch sources
- Browse hierarchy

### Notifications (Observable - ReactiveX)
- State changes (Playing, Paused, Streaming, ...)
- Mode changes (Shuffle, Repeat)
- Volume changes
- Position changes (note: not realtime)
- Media changes
- Preset changes
- Queue changes

## Getting started

```cs
using Blu4Net;
using System;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace GetttingStarted
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // find endpoint of player (first one)
            var endpoint = await BluEnvironment.ResolveEndpoints().FirstOrDefaultAsync();
            Console.WriteLine($"Endpoint: {endpoint}");

            // success?
            if (endpoint != null)
            {
                // yes, so create and connect the player
                var player = await BluPlayer.Connect(endpoint);
                Console.WriteLine($"Player: {player}");

                // get the state
                var state = await player.GetState();
                Console.WriteLine($"State: {state}");

                // get the volume
                var volume = await player.GetVolume();
                Console.WriteLine($"Volume: {volume}");

                // get the current playing media
                var media = await player.GetMedia();
                Console.WriteLine($"Media: {media.Titles.FirstOrDefault()}");

                // subscribe to volume changes
                using (player.VolumeChanges.Subscribe(volume => Console.WriteLine($"Volume: {volume}%")))
                {
                    Console.ReadLine();
                }
            }
            else
            {
                Console.WriteLine("No endpoint found!");
                Console.ReadLine();
            }
        }
    }
}
