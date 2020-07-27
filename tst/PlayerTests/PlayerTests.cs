using Blu4Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

namespace PlayerTests
{
    [TestClass]
    public class PlayerTests
    {
        static BluPlayer Player;

        [ClassInitialize()]
        public static async Task Initialize(TestContext testContext) 
        {
            Player = await BluEnvironment.ResolvePlayers().FirstAsync();
        }

        [TestMethod]
        public async Task Player_ChangeVolume()
        {
            // get the current volume
            var oldVolume = Player.Volume;

            // create an observerable which waits until the volume has changed by 1 percent
            var observerable = Player.VolumeChanges
                .Where(element => element == oldVolume + 1)
                .FirstAsync();

            // increase the volume by 1 percent
            await Player.SetVolume(oldVolume + 1);

            // wait until the observerable completes
            var newVolume = await observerable.Timeout(TimeSpan.FromSeconds(2));

            // restore old volume
            var restoredVolume = await Player.SetVolume(oldVolume);
        }
    }
}
