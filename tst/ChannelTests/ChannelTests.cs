using Blu4Net.Channel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace ChannelTests
{
    [TestClass]
    public class ChannelTests
    {
        public readonly Uri Endpoint = new Uri(@"http://192.168.0.27:11000");

        [TestMethod]
        public async Task Channel_GetSyncStatus()
        {
            var channel = new BluChannel(Endpoint);
            var response = await channel.GetSyncStatus();

            Assert.IsNotNull(response);
        }

        [TestMethod]
        public async Task Channel_GetStatus()
        {
            var channel = new BluChannel(Endpoint);
            var response = await channel.GetStatus();

            Assert.IsNotNull(response);
        }
    }
}
