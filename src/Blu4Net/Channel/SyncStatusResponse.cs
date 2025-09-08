using System;
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

        // Properties for when the player is in sync group
        [XmlElement("slave")]
        public Slave[] Slave = new Slave[0];

        [XmlElement("master")]
        public Master Master;

        // Properties for when the player is part of a zone
        [XmlAttribute("zoneController")]
        public bool IsZoneController;

        [XmlAttribute("zone")]
        public string ZoneName;

        [XmlAttribute("zoneUngroup")]
        public string ZoneUngroupUrl;

        [XmlIgnore]
        public ChannelMode? ChannelMode
        {
            get
            {
                if (string.IsNullOrEmpty(ChannelName)) return null;
                if (Enum.TryParse<ChannelMode>(ChannelName, true, out var val)) return val;
                return null;
            }
        }
        [XmlAttribute("channelName")]
        public string ChannelName;

        [XmlElement("zoneSlave")]
        public ZoneSlave ZoneSlave;

        public override string ToString()
        {
            return Name;
        }
    }

    public class Master
    {
        [XmlAttribute("port")]
        public int Port;

        [XmlText]
        public string Address;

        public override string ToString() => $"{Address}:{Port}";
    }

    public class Slave
    {
        [XmlAttribute("port")]
        public int Port;

        [XmlAttribute("id")]
        public string Address;

        public override string ToString() => $"{Address}:{Port}";
    }

    public class ZoneSlave
    {
        [XmlAttribute("id")]
        public string Address;

        [XmlAttribute("port")]
        public int Port;

        [XmlAttribute("zoneSlave")]
        public bool IsZoneSlave;

        [XmlIgnore]
        public ChannelMode? ChannelMode
        {
            get
            {
                if (string.IsNullOrEmpty(ChannelName)) return null;
                if (Enum.TryParse<ChannelMode>(ChannelName, true, out var val)) return val;
                return null;
            }
        }
        [XmlAttribute("channelName")]
        public string ChannelName;

        [XmlAttribute("name")]
        public string Name;

        [XmlAttribute("model")]
        public string Model;

        [XmlAttribute("modelName")]
        public string ModelName;

        public override string ToString() => $"{Name} ({Address}:{Port})";
    }
}
