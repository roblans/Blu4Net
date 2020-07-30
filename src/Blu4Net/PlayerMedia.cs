using Blu4Net.Channel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;

namespace Blu4Net
{
    public class PlayerMedia
    {
        public IReadOnlyList<string> Titles { get; }
        public Uri ImageUri { get; }
        public Uri ServiceIconUri { get; }

        public PlayerMedia(IReadOnlyList<string> titles, Uri imageUri, Uri serviceIconUri)
        {
            if (titles == null)
                throw new ArgumentNullException(nameof(titles));

            Titles = titles;
            ImageUri = imageUri;
            ServiceIconUri = serviceIconUri;
        }

        public override string ToString()
        {
            return Titles.FirstOrDefault() ?? base.ToString();
        }
    }
}
