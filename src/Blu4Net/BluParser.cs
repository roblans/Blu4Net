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
            if (value != null && Uri.TryCreate(value, UriKind.RelativeOrAbsolute, out var uri))
            {
                if (uri.IsAbsoluteUri)
                {
                    return uri;
                }
                return new Uri(baseUri, uri);
            }
            return null;
        }

        public static PlayerMedia ParseMedia(StatusResponse response, Uri endpoint)
        {
            if (response == null)
                throw new ArgumentNullException(nameof(response));

            var imageUri = response.Image != null ? ParseAbsoluteUri(response.Image, endpoint) : null;
            var serviceIconUri = response.ServiceIcon != null ? ParseAbsoluteUri(response.ServiceIcon, endpoint) : null;
            var titles = new[] { response.Title1, response.Title2, response.Title3 }.Where(element => element != null).ToArray();

            return new PlayerMedia(titles, imageUri, serviceIconUri);
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

        public static PlayPosition ParsePosition(StatusResponse response)
        {
            return new PlayPosition(TimeSpan.FromSeconds(response.Seconds), response.TotalLength != 0 ? TimeSpan.FromSeconds(response.TotalLength) : default(TimeSpan?));
        }

        public static MusicSource ParseMusicSource(BrowseContentResponse.Item item, BluChannel channel)
        {
            var source = new MusicSource(channel, item.BrowseKey);
            source.Load(item);
            return source;
        }

        public static MusicSourceEntry ParseMusicSourceEntry(BrowseContentResponse.Item item, BluChannel channel, MusicSourceEntry parent)
        {
            var source = new MusicSourceEntry(channel, item.BrowseKey, parent);
            source.Load(item);
            return source;
        }
    }
}
