using Blu4Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;

namespace PlayerTests
{
    [TestClass]
    public class PlayerTests
    {
        public IPAddress Address = IPAddress.Parse("192.168.0.27");

        [TestMethod]
        public async Task Player_GetSyncStatus()
        {
            var player = Player.Connect(Address);
            var response = await player.GetSyncStatus();

            Assert.IsNotNull(response);
        }

        [TestMethod]
        public async Task Player_GetStatus()
        {
            var player = Player.Connect(Address);
            var response = await player.GetStatus();

            Assert.IsNotNull(response);
        }
    }
}
