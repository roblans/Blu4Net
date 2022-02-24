using Blu4Net.Channel;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Blu4Net
{
    public class MusicContentCategory
    {
        public MusicContentCategory(BluChannel channel, MusicContentNode parent, BrowseContentResponse.Category response)
        {
            if (response == null)
                throw new ArgumentNullException(nameof(response));

            Name = response.Text;
            Entries = response.Items != null ? response.Items.Select(element => new MusicContentEntry(channel, parent, element)).ToArray() : new MusicContentEntry[0];
        }

        public IReadOnlyCollection<MusicContentEntry> Entries { get; }

        public string Name { get; }
    }
}