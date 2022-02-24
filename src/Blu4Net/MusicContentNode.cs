using Blu4Net.Channel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blu4Net
{
    public class MusicContentNode
    {
        private readonly BluChannel _channel;
        private readonly string _searchKey;

        public MusicContentNode Parent { get; }
        public string ServiceName { get; } 
        public Uri ServiceIconUri { get; } 
        public IReadOnlyCollection<MusicContentEntry> Entries { get; }
        public IReadOnlyCollection<MusicContentCategory> Categories { get; }

        public MusicContentNode(BluChannel channel, MusicContentNode parent, BrowseContentResponse response)
        {
            _channel = channel ?? throw new ArgumentNullException(nameof(channel));
            Parent = parent;
            if (response == null)
                throw new ArgumentNullException(nameof(response));

            _searchKey = response.SearchKey;
            ServiceName = response.ServiceName;
            ServiceIconUri = BluParser.ParseAbsoluteUri(response.ServiceIcon, _channel.Endpoint);
            Entries = response.Items != null ? response.Items.Select(element => new MusicContentEntry(channel, this, element)).ToArray() : new MusicContentEntry[0];
            Categories = response.Categories != null ? response.Categories.Select(category => new MusicContentCategory(channel, this, category)).ToArray() : new MusicContentCategory[0];
        }

        public bool IsSearchable
        {
            get { return _searchKey != null; }
        }

        public async Task<MusicContentNode> Search(string searchTerm)
        {
            if (_searchKey == null)
                throw new NotSupportedException("Musicsource is not searchable");

            var response = await _channel.BrowseContent(_searchKey, searchTerm).ConfigureAwait(false);
            return new MusicContentNode(_channel, this, response);
        }

        public override string ToString()
        {
            return ServiceName;
        }
    }
}
