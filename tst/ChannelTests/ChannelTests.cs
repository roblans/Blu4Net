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
            Assert.IsNotNull(response);
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
            Assert.IsNotNull(response);
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
            Assert.IsNotNull(response);
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
            Assert.IsNotNull(response);
        }

        [TestMethod]
        public async Task Channel_MuteOn()
        {
            var channel = new BluChannel(Enpoint);
            var response = await channel.Mute(true);
            Assert.IsNotNull(response);
        }

        [TestMethod]
        public async Task Channel_MuteOff()
        {
            var channel = new BluChannel(Enpoint);
            var response = await channel.Mute(false);
            Assert.IsNotNull(response);
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
            Assert.AreEqual(status.Length, listing.Songs.Length);
        }

        [TestMethod]
        public async Task Channel_GetPlayQueueListingSliced()
        {
            var songs = new List<PlayQueueSong>();
            var index  = 0;
            var length = 100;

            var channel = new BluChannel(Enpoint);
            while (true)
            {
                var listing = await channel.GetPlayQueueListing(index, length);
                if (listing.Songs == null)
                    break;
                
                songs.AddRange(listing.Songs);
                index += listing.Songs.Length;
            }

            var status = await channel.GetPlayQueueStatus();
            Assert.AreEqual(status.Length, songs.Count);
        }
    }
}
