using Blu4Net;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace BluDumper
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var endpoint = await BluEnvironment.ResolveEndpoints().FirstOrDefaultAsync();
            if (endpoint != null)
            {
                Console.WriteLine($"Endpoint: {endpoint}");

                var player = await BluPlayer.Connect(endpoint);
                //player.Log = Console.Out;
                
                Console.WriteLine($"Player: {player.Name}");
                Console.WriteLine(new string('=', 80));

                await DumpPlayer(player);

                while (true)
                {
                    var key = Console.ReadKey();

                    if (key.KeyChar == 'q')
                        break;

                    if (key.KeyChar == 'p')
                    {
                        await DumpQueuedSongs(player.PlayQueue);
                    }
                }
            }
            else 
            {
                Console.WriteLine("No player found!");
                Console.WriteLine("Press 'q' to quit");
                while (Console.ReadKey().KeyChar != 'q') { }
            }
        }

        private static async Task DumpPlayer(BluPlayer player)
        {
            Console.WriteLine($"State: {await player.GetState()}");
            Console.WriteLine($"Shuffle: {await player.GetShuffleMode()}");
            Console.WriteLine($"Repeat: {await player.GetRepeatMode()}");
            Console.WriteLine($"Volume: {await player.GetVolume()}%");

            DumpPresets(await player.PresetList.GetPresets());
            DumpMedia(await player.GetMedia());
            DumpQueueInfo(await player.PlayQueue.GetInfo());
            await DumpMusicSources(await player.GetMusicSources());

            Console.WriteLine(new string('=', 80));
            Console.WriteLine();

            Console.WriteLine("Waiting for changes...");
            Console.WriteLine($"Press 'q' to quit");
            Console.WriteLine($"Press 'p' to dump the PlayQueue");

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

            player.PresetList.Changes.Subscribe(presets =>
            {
                DumpPresets(presets);
            });

            player.PlayQueue.Changes.Subscribe(info =>
            {
                DumpQueueInfo(info);
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

        private static void DumpPresets(IReadOnlyCollection<PlayerPreset> presets)
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

        private static void DumpQueueInfo(PlayQueueInfo info)
        {
            Console.WriteLine($"Queue:");
            Console.WriteLine($"\tName: {info.Name}");
            Console.WriteLine($"\tLength: {info.Length}");
        }

        private static async Task DumpQueuedSongs(PlayQueue queue)
        {
            Console.WriteLine($"\tSongs:");
            await foreach (var page in queue.GetSongs(500))
            {
                foreach (var song in page)
                {
                    Console.WriteLine($"\t\t{song}");
                }
            }
            Console.WriteLine($"Done.");
        }

        private static async Task DumpMusicSources(IEnumerable<PlayerMusicSource> sources)
        {
            Console.WriteLine($"Sources (one level only):");
            foreach(var source in sources)
            {
                Console.WriteLine($"\t{source}");
                await DumpMusicSourceItems(await source.GetItems(), 1);
            }
        }

        private static async Task DumpMusicSourceItems(IReadOnlyCollection<PlayerMusicSourceItem> items, int maxLevels, int level = 0)
        {
            foreach (var item in items)
            {
                Console.WriteLine($"{new string('\t', level + 2)}{item}");

                if (item.HasItems && level < maxLevels - 1)
                {
                    var children = await item.GetItems();
                    await DumpMusicSourceItems(children, maxLevels, level + 1);
                }
            }
        }
    }
}
