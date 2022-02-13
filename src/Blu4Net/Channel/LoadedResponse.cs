using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Blu4Net.Channel
{
    public class LoadedResponse
    {
    }

    [XmlRoot("loaded")]
    public class PlaylistLoadedResponse : LoadedResponse
    {
        [XmlAttribute("service")]
        public string Service;

        [XmlElement("entries")]
        public int Entries;

        public override string ToString()
        {
            return $"{Service} {Entries}";
        }
    }

    [XmlRoot("state")]
    public class StreamLoadedResponse : LoadedResponse
    {
        [XmlText()]
        public string State;

        public override string ToString()
        {
            return State;
        }
    }
}
