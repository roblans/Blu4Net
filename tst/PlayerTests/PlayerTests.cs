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

        [ClassCleanup]
        public static void Cleanup()
        {
            Player.Dispose();
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

            // volume changed?
            Assert.AreNotEqual(oldVolume, newVolume);

            // restore old volume
            var restoredVolume = await Player.SetVolume(oldVolume);

            // volume restored?
            Assert.AreEqual(oldVolume, restoredVolume);
        }

        [TestMethod]
        public async Task Player_PausePlay()
        {
            if (Player.State == PlayerState.Playing)
            {
                // pause the player
                var oldState = await Player.Pause();

                // player paused?
                Assert.AreEqual(PlayerState.Paused, oldState);

                // create an observerable which waits until the player starts playing
                var observerable = Player.StateChanges
                    .Where(element => element == PlayerState.Playing)
                    .FirstAsync();

                // start playing
                await Player.Play();

                // wait until the observerable completes
                var newState = await observerable.Timeout(TimeSpan.FromSeconds(2));

                // player playing?
                Assert.AreEqual(PlayerState.Playing, newState);
            }
        }

        [TestMethod]
        public async Task Player_ChangeShuffleMode()
        {
            // get the current volume
            var oldMode = Player.Mode;

            // turn shuffle off
            var mode = await Player.SetMode(new PlayerMode(ShuffleMode.ShuffleOff, RepeatMode.RepeatOff));

            // shuffle off?
            Assert.AreEqual(mode.Shuffle, ShuffleMode.ShuffleOff);

            // create an observerable which waits until the shuffle is on
            var observerable = Player.ModeChanges
                .Where(element => element.Shuffle == ShuffleMode.ShuffleOn)
                .FirstAsync();


            // turn shuffle on
            mode = await Player.SetMode(new PlayerMode(ShuffleMode.ShuffleOn, RepeatMode.RepeatOff));

            // wait until the observerable completes
            var newState = await observerable.Timeout(TimeSpan.FromSeconds(20));

            // restore
            await Player.SetMode(oldMode);
        }

        [TestMethod]
        public async Task Player_ChangeRepeatMode()
        {
            // get the current volume
            var oldMode = Player.Mode;

            // turn repeat off
            var mode = await Player.SetMode(new PlayerMode(ShuffleMode.ShuffleOff, RepeatMode.RepeatOff));

            // repeat off?
            Assert.AreEqual(mode.Repeat, RepeatMode.RepeatOff);

            // create an observerable which waits until the repeat is on
            var observerable = Player.ModeChanges
                .Where(element => element.Repeat == RepeatMode.RepeatAll)
                .FirstAsync();


            // turn repeat on
            mode = await Player.SetMode(new PlayerMode(ShuffleMode.ShuffleOn, RepeatMode.RepeatAll));

            // wait until the observerable completes
            var newState = await observerable.Timeout(TimeSpan.FromSeconds(20));

            // restore
            await Player.SetMode(oldMode);
        }
    }
}
