using Blu4Net.Channel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blu4Net
{
    public class MusicSourceEntryInfo
    {
        public string Key { get; private set; }
        public string Name { get; private set; }
        public string Type { get; private set; }
        public Uri ImageUri { get; private set; }

        public MusicSourceEntryInfo(BrowseContentResponse.Item item, Uri endpoint)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            if (endpoint == null)
                throw new ArgumentNullException(nameof(endpoint));

            Key = item.BrowseKey;
            Name = item.Text;
            Type = !string.IsNullOrEmpty(item.Type) ? item.Type.First().ToString().ToUpper() + item.Type.Substring(1) : null;
            ImageUri = BluParser.ParseAbsoluteUri(item.Image, endpoint);
        }

        public bool IsContainer
        {
            get { return Key != null; }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
