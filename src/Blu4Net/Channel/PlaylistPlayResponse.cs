using System.Xml.Serialization;

namespace Blu4Net.Channel
{
    [XmlRoot("playlist")]
    public class PlaylistPlayResponse : LoadedResponse
    {
        [XmlAttribute("id")]
        public int ID;

        [XmlAttribute("length")]
        public int Length;

        [XmlAttribute("count")]
        public int Count;

        [XmlAttribute("shuffle")]
        public int Shuffle;

        [XmlAttribute("repeat")]
        public int Repeat;

        public override string ToString()
        {
            return $"Playlist {ID} (length={Length}, count={Count})";
        }
    }
}

