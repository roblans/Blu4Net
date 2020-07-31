using Blu4Net.Channel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blu4Net
{
    public class PlayerMusicSources
    {
        private readonly BluChannel _channel;

        internal PlayerMusicSources(BluChannel channel)
        {
            _channel = channel ?? throw new ArgumentNullException(nameof(channel));
        }

        public async Task<IReadOnlyCollection<PlayerMusicSourceItem>> GetItems(string parentKey = null)
        {
            var response = await _channel.BrowseContent(parentKey);

            return response.Items
                .Where(element => element.Text != null)
                .Select(element => new PlayerMusicSourceItem(element.Text, element.BrowseKey))
                .ToArray();
        }
    }
}
