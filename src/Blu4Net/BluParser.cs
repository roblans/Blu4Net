using Blu4Net.Channel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blu4Net
{
    public static class BluParser
    {
        public static Uri ParseAbsoluteUri(string value, Uri baseUri)
        {
            if (!string.IsNullOrEmpty(value) && Uri.TryCreate(value, UriKind.RelativeOrAbsolute, out var uri))
            {
                if (uri.IsAbsoluteUri)
                {
                    return uri;
                }
                return new Uri(baseUri, uri);
            }
            return null;
        }

        public static PlayerState ParseState(string value)
        {
            if (value != null)
            {
                switch (value)
                {
                    case "stream":
                        return PlayerState.Streaming;
                    case "play":
                        return PlayerState.Playing;
                    case "pause":
                        return PlayerState.Paused;
                    case "stop":
                        return PlayerState.Stopped;
                    case "connecting":
                        return PlayerState.Connecting;
                }
            }
            return PlayerState.Unknown;
        }

        public static MusicSource ParseMusicSource(BrowseContentResponse.Item item, BluChannel channel)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            if (channel == null)
                throw new ArgumentNullException(nameof(channel));

            var source = new MusicSource(channel, item.BrowseKey);
            source.Load(item);
            return source;
        }

        public static MusicSourceEntry ParseMusicSourceEntry(BrowseContentResponse.Item item, BluChannel channel, MusicSourceEntry parent)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            if (channel == null)
                throw new ArgumentNullException(nameof(channel));
            if (parent == null)
                throw new ArgumentNullException(nameof(parent));

            var source = new MusicSourceEntry(channel, item.BrowseKey, parent);
            source.Load(item);
            return source;
        }
    }
}
