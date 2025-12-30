using System;

namespace Blu4Net
{
    public class InputSourceItem
    {
        public string Id { get; }

        public string Name { get; }

        public Uri Image { get; }

        public string PlayUrl { get; }

        public string Key { get; }

        public bool IsActive { get; }

        public string Type { get; }

        internal InputSourceItem(Channel.RadioBrowseResponse.Item item, Uri endpoint)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));

            Id = item.Id;
            Name = item.Text;
            PlayUrl = item.Url;
            Key = item.Key;
            IsActive = item.IsActive == 1;
            Type = item.Type;

            if (!string.IsNullOrEmpty(item.Image))
            {
                if (Uri.TryCreate(item.Image, UriKind.Absolute, out var absoluteUri))
                {
                    Image = absoluteUri;
                }
                else if (endpoint != null)
                {
                    var builder = new UriBuilder(endpoint)
                    {
                        Path = item.Image
                    };
                    Image = builder.Uri;
                }
            }
        }

        public override string ToString()
        {
            return Name ?? Key ?? base.ToString();
        }
    }
}

