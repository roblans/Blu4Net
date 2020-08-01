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
                    Console.WriteLine();
                    Console.WriteLine($"Waiting for volume changes...");
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
