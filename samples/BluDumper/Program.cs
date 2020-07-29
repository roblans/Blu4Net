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

            var player = await BluPlayer.Connect(endpoint);
            await DumpPlayer(player);
        }

        private static async Task DumpPlayer(BluPlayer player)
        {
            Console.WriteLine($"Player: {player.Name}");
            Console.WriteLine(new string('=', 80));

            Console.WriteLine($"State: {player.State}");
            Console.WriteLine($"Mode: {player.Mode}");
            Console.WriteLine($"Volume: {player.Volume}%");
            
            Console.WriteLine($"Media:");
            for(var i=0; i<player.Media.Titles.Length; i++)
            {
                Console.WriteLine($"\tTitle{i+1}: {player.Media.Titles[i]}");
            }
            Console.WriteLine($"\tImageUri: {player.Media.ImageUri}");
            Console.WriteLine($"\tServiceIconUri: {player.Media.ServiceIconUri}");

            Console.WriteLine(new string('=', 80));
            Console.WriteLine();
        }
    }
}
