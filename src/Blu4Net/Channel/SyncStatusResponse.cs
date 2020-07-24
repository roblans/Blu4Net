using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Blu4Net.Channel
{
    [XmlRoot("SyncStatus")]
    public class SyncStatusResponse : LongPollingResponse
    {
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
    }
}
