using System;
using System.Collections.Generic;
using System.Text;

namespace Blu4Net
{
    public class PlayQueueSong
    {
        public int ID { get; }
        public string Artist { get; }
        public string Album { get; }
        public string Title { get; }

        public PlayQueueSong(int id, string artist, string album, string title)
        {
            ID = id;
            Artist = artist ?? throw new ArgumentNullException(nameof(artist));
            Album = album ?? throw new ArgumentNullException(nameof(album));
            Title = title ?? throw new ArgumentNullException(nameof(title));
        }

        public override string ToString()
        {
            return $"{Artist} | {Album} | {Title}";
        }
    }
}
