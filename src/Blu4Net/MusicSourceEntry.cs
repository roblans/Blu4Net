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
        
        public string ServiceName { get; } 
        public IReadOnlyCollection<MusicSourceEntryInfo> EntryInfos { get; }

        public MusicSourceEntry(BluChannel channel, BrowseContentResponse response)
        {
            _channel = channel ?? throw new ArgumentNullException(nameof(channel));

            if (response == null)
                throw new ArgumentNullException(nameof(response));

            ServiceName = response.ServiceName;
            EntryInfos = response.Items != null ? response.Items.Select(element => new MusicSourceEntryInfo(element, channel.Endpoint)).ToArray() : new MusicSourceEntryInfo[0];
        }


        public async Task<MusicSourceEntry> FetchEntry(string key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            var response = await _channel.BrowseContent(key);
            return new MusicSourceEntry(_channel, response);
        }

        public override string ToString()
        {
            return ServiceName;
        }
    }
}
