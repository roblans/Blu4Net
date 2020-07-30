using Blu4Net;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace BluDumper
{
    class Program
    {

        static async Task Main(string[] args)
        {
            var players = new List<BluPlayer>();

            using (BluEnvironment.ResolveEndpoints().Subscribe(async endpoint =>
            {
                Console.WriteLine($"Endpoint: {endpoint}");
     
                var player = new BluPlayer(endpoint);
                await player.Connect();
                
                DumpPlayer(player);

                players.Add(player);
            }))
            {
                Console.ReadLine();

                foreach(var player in players)
                {
                    await player.Disconnect();
                }
            }
        }

        private static void DumpPlayer(BluPlayer player)
        {
            Console.WriteLine($"Player: {player.Name}");
            Console.WriteLine(new string('=', 80));

            Console.WriteLine($"State: {player.State}");
            Console.WriteLine($"Mode: {player.Mode}");
            Console.WriteLine($"Volume: {player.Volume}%");

            DumpPresets(player.Presets);
            DumpMedia(player.Media);
            DumpQueue(player.Queue);

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

            player.PresetsChanges.Subscribe(presets =>
            {
                DumpPresets(presets);
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

        private static void DumpPresets(IEnumerable<PlayerPreset> presets)
        {
            Console.WriteLine($"Presets:");
            foreach (var preset in presets)
            {
                Console.WriteLine($"\tNumber: {preset.Number}");
                Console.WriteLine($"\tName: {preset.Name}");
                Console.WriteLine($"\tImageUri: {preset.ImageUri}");
                Console.WriteLine();
            }
        }

        private static void DumpQueue(PlayQueue queue)
        {
            Console.WriteLine($"Queue:");
            Console.WriteLine($"\tName: {queue.Name}");
            Console.WriteLine($"\tLength: {queue.Length}");
        }
    }
}
