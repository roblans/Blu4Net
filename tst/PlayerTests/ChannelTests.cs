using Blu4Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.Linq;
using System.Threading.Tasks;
using Blu4Net.Channel;
using System;
using System.Reactive.Linq;

namespace PlayerTests
{
    [TestClass]
    public class ChannelTests
    {
        static Uri Enpoint = new UriBuilder("http", "192.168.0.27", 11000).Uri;

        [TestMethod]
        public async Task Player_ObserveStatusChanges()
        {
            var channel = new BluChannel(Enpoint);
            var response = await channel.StatusChanges.Timeout(TimeSpan.FromSeconds(1)).FirstOrDefaultAsync();
            Assert.IsNotNull(response);
        }

        [TestMethod]
        public async Task Player_ObserveSyncStatusChanges()
        {
            var channel = new BluChannel(Enpoint);
            var response = await channel.SyncStatusChanges.Timeout(TimeSpan.FromSeconds(1)).FirstOrDefaultAsync();
            Assert.IsNotNull(response);
        }
    }
}
