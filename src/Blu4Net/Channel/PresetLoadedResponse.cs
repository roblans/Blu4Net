using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Blu4Net.Channel
{
    public class PresetLoadedResponse
    {
    }

    [XmlRoot("loaded")]
    public class PlaylistPresetLoadedResponse : PresetLoadedResponse
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
    public class StreamPresetLoadedResponse : PresetLoadedResponse
    {
        [XmlText()]
        public string State;

        public override string ToString()
        {
            return State;
        }
    }
}
