using Blu4Net.Channel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blu4Net
{
    public class MusicSourceEntry
    {
        private readonly BluChannel _channel;

        public MusicSourceEntry Parent { get; }
        public string BrowseKey { get; }
        public string Text { get; private set; }
        public Uri ImageUri { get; private set; }

        public MusicSourceEntry(BluChannel channel, string browseKey, MusicSourceEntry parent)
        {
            _channel = channel ?? throw new ArgumentNullException(nameof(channel));
            BrowseKey = browseKey;
            Parent = parent;
        }

        internal void Load(BrowseContentResponse.Item item)
        {
            Text = item.Text;
            ImageUri = BluParser.ParseAbsoluteUri(item.Image, _channel.Endpoint);
        }

        public bool IsContainer
        {
            get { return BrowseKey != null; }
        }

        public async Task<IReadOnlyCollection<MusicSourceEntry>> GetEntries()
        {
            if (BrowseKey != null)
            {
                var response = await _channel.BrowseContent(BrowseKey);
                return response.Items.Select(element => BluParser.ParseMusicSourceEntry(element, _channel, this)).ToArray();
            }
            return new MusicSourceEntry[0];
        }

        public override string ToString()
        {
            return Text;
        }
    }
}
