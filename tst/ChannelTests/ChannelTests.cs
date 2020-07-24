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
        static Uri Enpoint = new UriBuilder("http", "192.168.0.27", 11000).Uri;

        [TestMethod]
        public async Task Channel_ObserveStatus()
        {
            var channel = new BluChannel(Enpoint);
            var response = await channel.StatusChanges.Timeout(TimeSpan.FromSeconds(1)).FirstOrDefaultAsync();
            Assert.IsNotNull(response);
        }

        [TestMethod]
        public async Task Channel_ObserveSyncStatus()
        {
            var channel = new BluChannel(Enpoint);
            var response = await channel.SyncStatusChanges.Timeout(TimeSpan.FromSeconds(1)).FirstOrDefaultAsync();
            Assert.IsNotNull(response);
        }

        [TestMethod]
        public async Task Channel_ObserveVolume()
        {
            var channel = new BluChannel(Enpoint);
            var response = await channel.VolumeChanges.Timeout(TimeSpan.FromSeconds(1)).FirstOrDefaultAsync();
            Assert.IsNotNull(response);
        }

        [TestMethod]
        public async Task Channel_Play()
        {
            var channel = new BluChannel(Enpoint);
            var response = await channel.Play();
            Assert.IsTrue(response.State == "stream" || response.State == "play");
        }

        [TestMethod]
        public async Task Channel_PlaySeek()
        {
            var channel = new BluChannel(Enpoint);
            var response = await channel.Play(30);
            Assert.IsNotNull(response);
        }

        [TestMethod]
        public async Task Channel_Pause()
        {
            var channel = new BluChannel(Enpoint);
            var response = await channel.Pause();
            Assert.AreEqual("pause", response.State);
        }

        [TestMethod]
        public async Task Channel_PauseToggle()
        {
            var channel = new BluChannel(Enpoint);
            var response = await channel.Pause(toggle: true);
            Assert.IsNotNull(response);
        }

        [TestMethod]
        public async Task Channel_Stop()
        {
            var channel = new BluChannel(Enpoint);
            var response = await channel.Stop();
            Assert.AreEqual("stop", response.State);
        }

        [TestMethod]
        public async Task Channel_Skip()
        {
            var channel = new BluChannel(Enpoint);
            var response = await channel.Skip();
            Assert.IsNotNull(response);
        }

        [TestMethod]
        public async Task Channel_Back()
        {
            var channel = new BluChannel(Enpoint);
            var response = await channel.Back();
            Assert.IsNotNull(response);
        }

        [TestMethod]
        public async Task Channel_SetVolume()
        {
            var channel = new BluChannel(Enpoint);
            var response = await channel.SetVolume(10);
            Assert.AreEqual(10, response.Volume);
        }

        [TestMethod]
        public async Task Channel_MuteOn()
        {
            var channel = new BluChannel(Enpoint);
            var response = await channel.Mute(true);
            Assert.AreEqual(1, response.Mute);
        }

        [TestMethod]
        public async Task Channel_MuteOff()
        {
            var channel = new BluChannel(Enpoint);
            var response = await channel.Mute(false);
            Assert.AreEqual(0, response.Mute);
        }

        [TestMethod]
        public async Task Channel_GetPlayQueueStatus()
        {
            var channel = new BluChannel(Enpoint);
            var response = await channel.GetPlayQueueStatus();
            Assert.IsNotNull(response);
        }

        [TestMethod]
        public async Task Channel_GetPlayQueueListing()
        {
            var channel = new BluChannel(Enpoint);
            var listing = await channel.GetPlayQueueListing();
            var status = await channel.GetPlayQueueStatus();
            Assert.AreEqual(status.Length, listing.Tracks.Length);
        }

        [TestMethod]
        public async Task Channel_GetPlayQueueListingSliced()
        {
            var tracks = new List<PlayQueueTrack>();
            var index  = 0;
            var length = 100;

            var channel = new BluChannel(Enpoint);
            while (true)
            {
                var listing = await channel.GetPlayQueueListing(index, length);
                if (listing.Tracks == null)
                    break;
                
                tracks.AddRange(listing.Tracks);
                index += listing.Tracks.Length;
            }

            var status = await channel.GetPlayQueueStatus();
            Assert.AreEqual(status.Length, tracks.Count);
        }
    }
}
