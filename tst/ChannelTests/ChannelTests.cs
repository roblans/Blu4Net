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
            var enpoint = await BluEnvironment.ResolveEndpoints().FirstAsync();
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
            var volume = (await Channel.GetVolume()).Volume;

            var response = Channel.StatusChanges
                .Where(element => element.Volume == volume + 1)
                .Timeout(TimeSpan.FromSeconds(10))
                .FirstAsync();

            await Channel.SetVolume(volume + 1);
        }

        [TestMethod]
        public async Task Channel_SyncStatusChanged()
        {
            var volume = (await Channel.GetVolume()).Volume;

            var response = Channel.SyncStatusChanges
                .Where(element => element.Volume == volume + 1)
                .Timeout(TimeSpan.FromSeconds(10))
                .FirstAsync();

            await Channel.SetVolume(volume + 1);
        }

        [TestMethod]
        public async Task Channel_VolumeChanged()
        {
            var volume = (await Channel.GetVolume()).Volume;

            var response = Channel.VolumeChanges
                .Where(element => element.Volume == volume + 1)
                .Timeout(TimeSpan.FromSeconds(10))
                .FirstAsync();

            await Channel.SetVolume(volume + 1);
        }

        [TestMethod]
        public async Task Channel_Play()
        {
            var status = await Channel.GetPlaylistStatus();
            if (status.Length > 0)
            {
                var response = await Channel.Play();
                Assert.AreEqual("play", response.State);
            }
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
            var response = await Channel.Pause(toggle: 1);
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
            var response = await Channel.Mute(1);
            Assert.AreEqual(1, response.Mute);
        }

        [TestMethod]
        public async Task Channel_MuteOff()
        {
            var response = await Channel.Mute(0);
            Assert.AreEqual(0, response.Mute);
        }

        [TestMethod]
        public async Task Channel_GetPlaylistStatus()
        {
            var response = await Channel.GetPlaylistStatus();
            Assert.IsNotNull(response);
        }

        [TestMethod]
        public async Task Channel_GetPlaylist()
        {
            var response = await Channel.GetPlaylist();
            var status = await Channel.GetPlaylistStatus();
            Assert.AreEqual(status.Length, response.Songs.Length);
        }

        [TestMethod]
        public async Task Channel_GetPlaylistPaged()
        {
            var length = 0;
            await foreach (var list in Channel.GetPlaylistPaged(500))
            {
                length += list.Songs.Length;
            }
            var status = await Channel.GetPlaylistStatus();
            Assert.AreEqual(status.Length, length);
        }

        [TestMethod]
        public async Task Channel_ClearPlaylist()
        {
            var response = await Channel.ClearPlaylist();
            Assert.AreEqual(0, response.Length);
        }

        [TestMethod]
        public async Task Channel_GetShuffle()
        {
            var response = await Channel.GetShuffle();
            Assert.IsTrue(response.Shuffle >= 0 && response.Shuffle <= 1);
        }

        [TestMethod]
        public async Task Channel_ShuffleToggle()
        {
            var shuffle = (await Channel.GetShuffle()).Shuffle;
            var response = await Channel.SetShuffle(shuffle == 0 ? 1 : 0);
            Assert.AreNotEqual(shuffle, response.Shuffle);
        }

        [TestMethod]
        public async Task Channel_GetRepeat()
        {
            var response = await Channel.GetRepeat();
            Assert.IsTrue(response.Repeat >= 0 && response.Repeat <= 2);
        }


        [TestMethod]
        public async Task Channel_RepeatToggle()
        {
            var repeat = (await Channel.GetRepeat()).Repeat;
            var response = await Channel.SetRepeat((repeat + 1) % 3);
            Assert.AreNotEqual(repeat, response.Repeat);
        }

        [TestMethod]
        public async Task Channel_GetPresets()
        {
            var response = await Channel.GetPresets();
            Assert.IsTrue(response.Presets.Length >= 0 && response.Presets.Length <= 40);
        }

        [TestMethod]
        public async Task Channel_LoadPreset()
        {
            var preset2 = await Channel.LoadPreset(2);
            var preset5 = await Channel.LoadPreset(5);
        }

        [TestMethod]
        public async Task Channel_BrowseContent()
        {
            var root = await Channel.BrowseContent();

            var libraryKey = root.Items.FirstOrDefault(element => element.Text == "Library");
            if (libraryKey != null)
            {
                var library = await Channel.BrowseContent(libraryKey.BrowseKey);
                var artistsItem = library.Items.FirstOrDefault(element => element.Text == "Artists");
                if (artistsItem != null)
                {
                    var alphabet = await Channel.BrowseContent(artistsItem.BrowseKey);
                    var letterMItem = alphabet.Items.FirstOrDefault(element => element.Text == "M");
                    if (letterMItem != null)
                    {
                        var artists = await Channel.BrowseContent(letterMItem.BrowseKey);
                        var museItem = artists.Items.FirstOrDefault(element => element.Text == "Muse");
                        if (museItem != null)
                        {
                            var category = await Channel.BrowseContent(museItem.BrowseKey);
                            var albumsItem = category.Items.FirstOrDefault(element => element.Text == "Albums");
                            if (albumsItem != null)
                            {
                                var albums = await Channel.BrowseContent(albumsItem.BrowseKey);
                                var albumItem = albums.Items.FirstOrDefault(element => element.Text == "Black Holes and Revelations");
                                if (albumItem != null)
                                {
                                    var tracks = await Channel.BrowseContent(albumItem.BrowseKey);
                                    var trackItem = tracks.Items.FirstOrDefault(element => element.Text == "11. Knights of Cydonia");
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
