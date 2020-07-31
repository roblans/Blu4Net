using Blu4Net.Channel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blu4Net
{
    public class PlayerMusicSourceItem
    {
        private readonly BluChannel _channel;

        public string Key { get; }
        public string Text { get; }

        public PlayerMusicSourceItem(BluChannel channel, string key, string text)
        {
            _channel = channel ?? throw new ArgumentNullException(nameof(channel));
            Key = key;
            Text = text ?? throw new ArgumentNullException(nameof(text));
        }

        public bool HasItems
        {
            get { return Key != null; }
        }

        public async Task<IReadOnlyCollection<PlayerMusicSourceItem>> GetItems()
        {
            if (Key != null)
            {
                var response = await _channel.BrowseContent(Key);
                return response.Items.Select(element => new PlayerMusicSourceItem(_channel, element.BrowseKey, element.Text)).ToArray();
            }
            return new PlayerMusicSourceItem[0];
        }

        public override string ToString()
        {
            return Text;
        }
    }
}
