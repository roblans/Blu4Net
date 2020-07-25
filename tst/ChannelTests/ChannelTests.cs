using Blu4Net;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using System.Linq;
using System.Threading.Tasks;
using Blu4Net.Channel;
using System;
using System.Reactive.Linq;
using System.Collections.Generic;
using System.Diagnostics;

namespace ChannelTests
{
    [TestClass]
    public class ChannelTests
    {
        static BluChannel Channel;

        [ClassInitialize()]
        public static async Task Initialize(TestContext testContext)
        {
            var enpoint = await BluEnvironment.FindPlayerEndpoints().FirstAsync();
            Channel = new BluChannel(enpoint);
        }

        [TestMethod]
        public async Task Channel_GetStatus()
        {
            var response = await Channel.GetStatus();
            Assert.IsNotNull(response);
        }

        [TestMethod]
        public async Task Channel_GetSyncStatus()
        {
            var response = await Channel.GetSyncStatus();
            Assert.IsNotNull(response);
        }

        [TestMethod]
        public async Task Channel_StatusChanged()
        {
            var response = Channel.StatusChanges
                .Timeout(TimeSpan.FromSeconds(10))
                .FirstAsync();

            await response;
        }

        [TestMethod]
        public async Task Channel_SyncStatusChanged()
        {
            var response = Channel.SyncStatusChanges
                .Timeout(TimeSpan.FromSeconds(10))
                .FirstAsync();

            await response;
        }

        [TestMethod]
        public async Task Channel_VolumeChanged()
        {
            var response = Channel.VolumeChanges
                .Timeout(TimeSpan.FromSeconds(10))
                .FirstAsync();

            await response;
        }

        [TestMethod]
        public async Task Channel_Play()
        {
            var response = await Channel.Play();
            Assert.IsTrue(response.State == "stream" || response.State == "play");
        }

        [TestMethod]
        public async Task Channel_PlaySeek()
        {
            var response = await Channel.Play(30);
            Assert.IsNotNull(response);
        }

        [TestMethod]
        public async Task Channel_Pause()
        {
            var response = await Channel.Pause();
            Assert.AreEqual("pause", response.State);
        }

        [TestMethod]
        public async Task Channel_PauseToggle()
        {
            var response = await Channel.Pause(toggle: true);
            Assert.IsNotNull(response);
        }

        [TestMethod]
        public async Task Channel_Stop()
        {
            var response = await Channel.Stop();
            Assert.AreEqual("stop", response.State);
        }

        [TestMethod]
        public async Task Channel_Skip()
        {
            var status = await Channel.GetStatus();
            if (status.State == "play")
            {
                var response = await Channel.Skip();
                Assert.IsNotNull(response);
            }
        }

        [TestMethod]
        public async Task Channel_Back()
        {
            var status = await Channel.GetStatus();
            if (status.State == "play")
            {
                var response = await Channel.Back();
                Assert.IsNotNull(response);
            }
        }

        [TestMethod]
        public async Task Channel_GetVolume()
        {
            var response = await Channel.GetVolume();
            Assert.IsNotNull(response);
        }

        [TestMethod]
        public async Task Channel_SetVolume()
        {
            var response = await Channel.SetVolume(10);
            Assert.AreEqual(10, response.Volume);
        }

        [TestMethod]
        public async Task Channel_MuteOn()
        {
            var response = await Channel.Mute(true);
            Assert.AreEqual(1, response.Mute);
        }

        [TestMethod]
        public async Task Channel_MuteOff()
        {
            var response = await Channel.Mute(false);
            Assert.AreEqual(0, response.Mute);
        }

        [TestMethod]
        public async Task Channel_GetPlayQueueStatus()
        {
            var response = await Channel.GetPlayQueueStatus();
            Assert.IsNotNull(response);
        }

        [TestMethod]
        public async Task Channel_GetPlayQueueListing()
        {
            var listing = await Channel.GetPlayQueueListing();
            var status = await Channel.GetPlayQueueStatus();
            Assert.AreEqual(status.Length, listing.Tracks.Length);
        }

        [TestMethod]
        public async Task Channel_GetPlayQueueListingSliced()
        {
            var tracks = new List<PlayQueueTrack>();
            var index  = 0;
            var length = 100;

            while (true)
            {
                var listing = await Channel.GetPlayQueueListing(index, length);
                if (listing.Tracks.Length == 0)
                    break;
                
                tracks.AddRange(listing.Tracks);
                index += listing.Tracks.Length;
            }

            var status = await Channel.GetPlayQueueStatus();
            Assert.AreEqual(status.Length, tracks.Count);
        }
    }
}
