using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Xml.Serialization;

namespace Blu4Net.Channel
{
    [XmlRoot("volume")]
    public class VolumeResponse : ILongPollingResponse
    {
        [XmlAttribute("etag")]
        public string ETag { get; set; }

        [XmlAttribute("db")]
        public double Decibel;

        [XmlAttribute("mute")]
        public int Mute;

        [XmlText()]
        public int Volume;

        public override string ToString()
        {
            return $"{Volume}% {Decibel}db";
        }
    }
}
