using Blu4Net;
using Blu4Net.IO;
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
        public static async Task Initialize(TestContext context)
        {
            var endpoint = await BluEnvironment.ResolveEndpoints().FirstAsync();

            Player = await BluPlayer.Connect(endpoint);
            Player.Log = new DelegateTextWriter((message => context.WriteLine(message)));
        }


        [TestMethod]
        public async Task Player_ChangeVolume()
        {
            var previous = await Player.GetVolume();
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
            var state = await Player.GetState();
            if (state == PlayerState.Playing)
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
            var previous = await Player.GetShuffleMode();
            try
            {
                await Player.SetShuffleMode(ShuffleMode.ShuffleOff);

                var completion = new TaskCompletionSource<ShuffleMode>();

                using (Player.ShuffleModeChanges
                    .Where(element => element == ShuffleMode.ShuffleOn)
                    .Timeout(TimeSpan.FromSeconds(2))
                    .Subscribe(value =>
                    {
                        completion.SetResult(value);
                    }))
                {
                    await Player.SetShuffleMode(ShuffleMode.ShuffleOn);
                    await completion.Task;
                };
            }
            finally
            {
                await Player.SetShuffleMode(previous);
            }
        }

        [TestMethod]
        public async Task Player_ChangeRepeatMode()
        {
            var previous = await Player.GetRepeatMode();
            try
            {
                await Player.SetRepeatMode(RepeatMode.RepeatOff);

                var completion = new TaskCompletionSource<RepeatMode>();

                using (Player.RepeatModeChanges
                    .Where(element => element == RepeatMode.RepeatAll)
                    .Timeout(TimeSpan.FromSeconds(2))
                    .Subscribe(value =>
                    {
                        completion.SetResult(value);
                    }))
                {
                    await Player.SetRepeatMode(RepeatMode.RepeatAll);
                    await completion.Task;
                };
            }
            finally
            {
                await Player.SetRepeatMode(previous);
            }
        }

        [TestMethod]
        public async Task Player_Play()
        {
            await Player.Play();
        }

        [TestMethod]
        public async Task Player_Seek()
        {
            await Player.Seek(TimeSpan.FromSeconds(30));
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

        [TestMethod]
        public async Task Player_GetPresets()
        {
            var presets = await Player.PresetList.GetPresets();
            Assert.IsNotNull(presets);
        }
    }
}
