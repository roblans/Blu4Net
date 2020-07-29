using Blu4Net.Channel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blu4Net
{
    public class PlayerMedia : IEquatable<PlayerMedia>
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

        public override bool Equals(object obj)
        {
            return Equals(obj as PlayerMedia);
        }

        public bool Equals(PlayerMedia other)
        {
            return other != null && Titles.SequenceEqual(other.Titles);
        }

        public override int GetHashCode()
        {
            return Titles
                .Where(element => element != null)
                .Select(element => element.GetHashCode())
                .Aggregate(-1591799448, (sum, item) => sum + 1521134295 * item);
        }
    }
}
