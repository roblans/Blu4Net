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
        private readonly string _searchKey;

        public string ServiceName { get; } 
        public IReadOnlyCollection<MusicSourceEntryLink> Links { get; }

        public MusicSourceEntry(BluChannel channel, BrowseContentResponse response)
        {
            _channel = channel ?? throw new ArgumentNullException(nameof(channel));
            if (response == null)
                throw new ArgumentNullException(nameof(response));

            _searchKey = response.SearchKey;
            ServiceName = response.ServiceName;
            Links = response.Items != null ? response.Items.Select(element => new MusicSourceEntryLink(element, channel.Endpoint)).ToArray() : new MusicSourceEntryLink[0];
        }

        public async Task<MusicSourceEntry> ResolveLink(string key)
        {
            if (key == null)
                throw new ArgumentNullException(nameof(key));

            var response = await _channel.BrowseContent(key);
            return new MusicSourceEntry(_channel, response);
        }

        public bool IsSearchable
        {
            get { return _searchKey != null; }
        }

        public async Task<MusicSourceEntry> Search(string text)
        {
            if (_searchKey == null)
                throw new NotSupportedException("Musicsource is not searchable");

            var response = await _channel.BrowseContent(_searchKey, text);
            return new MusicSourceEntry(_channel, response);
        }

        public override string ToString()
        {
            return ServiceName;
        }
    }
}
