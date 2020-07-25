using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Blu4Net.Channel
{
    [XmlRoot("playlist")]
    public class PlayQueueStatusResponse
    {
        [XmlElement("name")]
        public string Name;

        [XmlElement("length")]
        public int Length;

        [XmlElement("modified")]
        public int Modified;

        public override string ToString()
        {
            return Name;
        }
    }
}
