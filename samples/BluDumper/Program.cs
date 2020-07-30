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

        static void Main(string[] args)
        {
            using (BluEnvironment.ResolveEndpoints().Subscribe(async endpoint =>
            {
                Console.WriteLine($"Endpoint: {endpoint}");
     
                var player = await BluPlayer.Connect(endpoint);
                
                await DumpPlayer(player);
            }))
            {
                Console.ReadLine();
            }
        }

        private static async Task DumpPlayer(BluPlayer player)
        {
            Console.WriteLine($"Player: {player.Name}");
            Console.WriteLine(new string('=', 80));

            Console.WriteLine($"State: {await player.GetState()}");
            Console.WriteLine($"Shuffle: {await player.GetShuffleMode()}");
            Console.WriteLine($"Repeat: {await player.GetRepeatMode()}");
            Console.WriteLine($"Volume: {await player.GetVolume()}%");

            await DumpPresets(player.PresetList);
            DumpMedia(await player.GetMedia());
            await DumpQueue(player.PlayQueue);

            Console.WriteLine(new string('=', 80));
            Console.WriteLine();

            Console.WriteLine("Waiting for changes...");
            player.StateChanges.Subscribe(state =>
            {
                Console.WriteLine($"State: {state}");
            });

            player.ShuffleModeChanges.Subscribe(mode =>
            {
                Console.WriteLine($"Shuffle: {mode}");
            });

            player.RepeatModeChanges.Subscribe(mode =>
            {
                Console.WriteLine($"Repeat: {mode}");
            });

            player.VolumeChanges.Subscribe(volume =>
            {
                Console.WriteLine($"Volume: {volume}%");
            });

            player.MediaChanges.Subscribe(media =>
            {
                DumpMedia(media);
            });

            player.PresetList.Changes.Subscribe(async _ =>
            {
                await DumpPresets(player.PresetList);
            });

            player.PlayQueue.Changes.Subscribe(async _ =>
            {
                await DumpQueue(player.PlayQueue);
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

        private static async Task DumpPresets(PresetList list)
        {
            Console.WriteLine($"Presets:");
            foreach (var preset in await list.GetPresets())
            {
                Console.WriteLine($"\tNumber: {preset.Number}");
                Console.WriteLine($"\tName: {preset.Name}");
                Console.WriteLine($"\tImageUri: {preset.ImageUri}");
                Console.WriteLine();
            }
        }

        private static async Task DumpQueue(PlayQueue queue)
        {
            Console.WriteLine($"Queue:");

            var info = await queue.GetInfo();
            Console.WriteLine($"\tName: {info.Name}");
            Console.WriteLine($"\tLength: {info.Length}");
        }
    }
}
