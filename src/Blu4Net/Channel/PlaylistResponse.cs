using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Blu4Net.Channel
{
    [XmlRoot("playlist")]
    public class PlaylistResponse
    {
        [XmlAttribute("name")]
        public string Name;

        [XmlAttribute("length")]
        public int Length;

        [XmlAttribute("modified")]
        public int Modified;

        [XmlElement("song")]
        public PlaylistTrack[] Tracks = new PlaylistTrack[0];

        public override string ToString()
        {
            return Name;
        }
    }

    [XmlRoot("song")]
    public class PlaylistTrack
    {
        [XmlElement("title")]
        public string Title;

        [XmlElement("art")]
        public string Artist;

        [XmlElement("alb")]
        public string Album;

        public override string ToString()
        {
            return Title;
        }
    }
}
