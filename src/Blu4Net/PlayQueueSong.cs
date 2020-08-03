using Blu4Net.Channel;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Text;

namespace Blu4Net
{
    public class PlayQueueSong
    {
        public int ID { get; }
        public string Artist { get; }
        public string Album { get; }
        public string Title { get; }

        public PlayQueueSong(PlaylistResponse.Song response)
        {
            if (response == null)
                throw new ArgumentNullException(nameof(response));

            ID = response.ID;
            Artist = response.Artist;
            Album = response.Album;
            Title = response.Title;
        }

        public override string ToString()
        {
            return $"{Artist} | {Album} | {Title}";
        }
    }
}
