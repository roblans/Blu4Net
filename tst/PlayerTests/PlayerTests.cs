using Blu4Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.Linq;
using System.Threading.Tasks;
using Blu4Net.Channel;
using System;

namespace PlayerTests
{
    [TestClass]
    public class PlayerTests
    {
        static Player _player;

        [ClassInitialize()]
        public static async Task Initialize(TestContext testContext) 
        {
            _player = (await Player.DiscoverPlayers()).First();
        }

        [TestMethod]
        public async Task Player_GetSyncStatus()
        {
            var response = await _player.GetSyncStatus();

            Assert.IsNotNull(response);
        }

        [TestMethod]
        public async Task Player_GetStatus()
        {
            var response = await _player.GetStatus();

            Assert.IsNotNull(response);
        }

        [TestMethod]
        public async Task Player_ObserveSyncStatus()
        {
            var channel = new BluChannel(new UriBuilder("http", "192.168.0.27", 11000).Uri);
            var subscription = channel.StartObserving<SyncStatus>("SyncStatus", 100).Subscribe(value =>
            {
                System.Diagnostics.Trace.WriteLine($"Volume: {value.VolumePercent}%, {value.VolumeDB}dB");
            });

            await Task.Delay(10000);
            subscription.Dispose();
        }
    }
}
