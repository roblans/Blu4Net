using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Blu4Net.Channel
{
    [XmlRoot("playlist")]
    public class ClearPlayQueueResponse
    {
        [XmlAttribute("length")]
        public int Length;

        [XmlAttribute("modified")]
        public int Modified;

        public override string ToString()
        {
            return Length.ToString();
        }
    }
}
