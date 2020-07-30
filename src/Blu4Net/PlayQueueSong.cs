using System;
using System.Collections.Generic;
using System.Text;

namespace Blu4Net
{
    public class PlayQueueSong
    {
        public string Artist { get; }
        public string Album { get; }
        public string Title { get; }

        public PlayQueueSong(string artist, string album, string title)
        {
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
