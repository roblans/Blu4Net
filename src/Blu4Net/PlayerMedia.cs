using System;
using System.Collections.Generic;
using System.Text;

namespace Blu4Net
{
    public class PlayerMedia
    {
        public string[] Titles { get; }
        public Uri ImageUri { get; }
        public Uri ServiceIconUri { get; }

        public PlayerMedia(string[] titles, Uri imageUri, Uri serviceIconUri)
        {
            Titles = titles ?? throw new ArgumentNullException(nameof(titles));
            ImageUri = imageUri;
            ServiceIconUri = serviceIconUri;
        }
    }
}
