using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Blu4Net.Channel
{
    [XmlRoot("SyncStatus")]
    public class SyncStatusResponse : ILongPollingResponse
    {
        [XmlAttribute("etag")]
        public string ETag { get; set; }

        [XmlAttribute("modelName")]
        public string ModelName;

        [XmlAttribute("name")]
        public string Name;

        [XmlAttribute("brand")]
        public string Brand;

        [XmlAttribute("volume")]
        public int Volume;

        [XmlAttribute("db")]
        public double Decibel;

        [XmlAttribute("mac")]
        public string MAC;

        public override string ToString()
        {
            return Name;
        }
    }
}
