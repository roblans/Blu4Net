using Blu4Net.Channel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blu4Net
{
    public class PlayerPreset
    {
        public int Number { get; }
        public string Name { get; }
        public Uri ImageUri { get; }

        public PlayerPreset(PresetsResponse.Preset response, Uri endpoint)
        {
            Number = response.ID;
            Name = response.Name;
            ImageUri = BluParser.ParseAbsoluteUri(response.Image, endpoint);
        }

        public override string ToString()
        {
            return $"{Number}. {Name}";
        }
    }
}
