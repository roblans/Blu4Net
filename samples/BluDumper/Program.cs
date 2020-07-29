using Blu4Net;
using System;
using System.Net;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace BluDumper
{
    class Program
    {
        static void Main(string[] args)
        {
            using (BluEnvironment.ResolveEndpoints()
                .Select(async endPoint => await DumpEndpoint(endPoint))
                .Subscribe())
            {
                Console.ReadLine();
            }
        }


        private static async Task DumpEndpoint(Uri endpoint)
        {
            Console.WriteLine($"Endpoint: {endpoint}");

            var player = new BluPlayer(endpoint);
            player.Log = Console.Out;
            await player.Connect();
            await DumpPlayer(player);
        }

        private static async Task DumpPlayer(BluPlayer player)
        {
            Console.WriteLine($"Player: {player.Name}");
            Console.WriteLine(new string('=', 80));

            Console.WriteLine($"State: {player.State}");
            Console.WriteLine($"Mode: {player.Mode}");
            Console.WriteLine($"Volume: {player.Volume}%");

            Console.WriteLine($"Presets:");
            foreach(var preset in await player.GetPresets())
            {
                Console.WriteLine($"\tNumber: {preset.Number}");
                Console.WriteLine($"\tName: {preset.Name}");
                Console.WriteLine($"\tImageUri: {preset.ImageUri}");
                Console.WriteLine();
            }

            DumpMedia(player.Media);

            Console.WriteLine(new string('=', 80));
            Console.WriteLine();

            Console.WriteLine("Waiting for changes...");
            player.StateChanges.Subscribe(state =>
            {
                Console.WriteLine($"State: {state}");
            });

            player.ModeChanges.Subscribe(mode =>
            {
                Console.WriteLine($"Mode: {mode}");
            });

            player.VolumeChanges.Subscribe(volume =>
            {
                Console.WriteLine($"Volume: {volume}%");
            });

            player.MediaChanges.Subscribe(media =>
            {
                DumpMedia(media);
            });
        }

        private static void DumpMedia(PlayerMedia media)
        {
            Console.WriteLine($"Media:");
            for (var i = 0; i < media.Titles.Count; i++)
            {
                Console.WriteLine($"\tTitle{i + 1}: {media.Titles[i]}");
            }
            Console.WriteLine($"\tImageUri: {media.ImageUri}");
            Console.WriteLine($"\tServiceIconUri: {media.ServiceIconUri}");
        }
    }
}
