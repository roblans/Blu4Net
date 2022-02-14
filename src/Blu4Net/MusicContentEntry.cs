using Blu4Net.Channel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blu4Net
{
    public class MusicContentEntry
    {
        private readonly BluChannel _channel;
        private readonly string _key;

        public MusicContentNode Node { get;}
        public string Name { get;}
        public string Text2 { get;}
        public string PlayURL { get; }
        public string AutoplayURL { get; }
        public string Type { get;}
        public Uri ImageUri { get; }

        public MusicContentEntry(BluChannel channel, MusicContentNode node, BrowseContentResponse.Item item)
        {
            _channel = channel ?? throw new ArgumentNullException(nameof(channel));
            Node = node ?? throw new ArgumentNullException(nameof(node));
            
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            _key = item.BrowseKey;
            Name = item.Text;
            Text2 = item.Text2;
            PlayURL = item.PlayURL;
            AutoplayURL = item.AutoplayURL;
            Type = !string.IsNullOrEmpty(item.Type) ? item.Type.First().ToString().ToUpper() + item.Type.Substring(1) : null;
            ImageUri = BluParser.ParseAbsoluteUri(item.Image, channel.Endpoint);
        }

        public bool IsResolvable
        {
            get { return _key != null; }
        }

        public async Task<MusicContentNode> Resolve()
        {
            if (_key == null)
                throw new InvalidOperationException("This entry is not resolvable");

            var response = await _channel.BrowseContent(_key).ConfigureAwait(false);
            return new MusicContentNode(_channel, Node, response);
        }


        public override string ToString()
        {
            return Name;
        }
    }
}
