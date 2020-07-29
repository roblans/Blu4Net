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
            var previous = Player.Volume;
            try
            {
                var completion = new TaskCompletionSource<int>();

                using (Player.VolumeChanges
                    .Where(element => element == previous + 1)
                    .Timeout(TimeSpan.FromSeconds(2))
                    .Subscribe(value =>
                    {
                        completion.SetResult(value);
                    }))
                {
                    await Player.SetVolume(previous + 1);
                    await completion.Task;
                };
            }
            finally
            {
                await Player.SetVolume(previous);
            }
        }

        [TestMethod]
        public async Task Player_ChangeState()
        {
            if (Player.State == PlayerState.Playing)
            {
                try
                {
                    var completion = new TaskCompletionSource<PlayerState>();

                    using (Player.StateChanges
                        .Where(element => element == PlayerState.Paused)
                        .Timeout(TimeSpan.FromSeconds(2))
                        .Subscribe(value =>
                        {
                            completion.SetResult(value);
                        }))
                    {
                        await Player.Pause();
                        await completion.Task;
                    };
                }
                finally
                {
                    await Player.Play();
                }
            }
        }

        [TestMethod]
        public async Task Player_ChangeShuffleMode()
        {
            var previous = Player.Mode;
            try
            {
                await Player.SetMode(new PlayerMode(ShuffleMode.ShuffleOff, RepeatMode.RepeatOff));

                var completion = new TaskCompletionSource<PlayerMode>();

                using (Player.ModeChanges
                    .Where(element => element.Shuffle == ShuffleMode.ShuffleOn)
                    .Timeout(TimeSpan.FromSeconds(2))
                    .Subscribe(value =>
                    {
                        completion.SetResult(value);
                    }))
                {
                    await Player.SetMode(new PlayerMode(ShuffleMode.ShuffleOn, RepeatMode.RepeatOff));
                    await completion.Task;
                };
            }
            finally
            {
                await Player.SetMode(previous);
            }
        }

        [TestMethod]
        public async Task Player_ChangeRepeatMode()
        {
            var previous = Player.Mode;
            try
            {
                await Player.SetMode(new PlayerMode(ShuffleMode.ShuffleOff, RepeatMode.RepeatOff));

                var completion = new TaskCompletionSource<PlayerMode>();

                using (Player.ModeChanges
                    .Where(element => element.Repeat == RepeatMode.RepeatAll)
                    .Timeout(TimeSpan.FromSeconds(2))
                    .Subscribe(value =>
                    {
                        completion.SetResult(value);
                    }))
                {
                    await Player.SetMode(new PlayerMode(ShuffleMode.ShuffleOff, RepeatMode.RepeatAll));
                    await completion.Task;
                };
            }
            finally
            {
                await Player.SetMode(previous);
            }
        }

        [TestMethod]
        public async Task Player_Play()
        {
            await Player.Play();
        }

        [TestMethod]
        public async Task Player_MuteOn()
        {
            await Player.Mute(true);
        }

        [TestMethod]
        public async Task Player_MuteOff()
        {
            await Player.Mute(false);
        }

        [TestMethod]
        public async Task Player_Stop()
        {
            await Player.Stop();
        }

        [TestMethod]
        public async Task Player_Pause()
        {
            await Player.Pause();
        }

        [TestMethod]
        public async Task Player_Skip()
        {
            await Player.Skip();
        }

        [TestMethod]
        public async Task Player_Back()
        {
            await Player.Back();
        }
    }
}
