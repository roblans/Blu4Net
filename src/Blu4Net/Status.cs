using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Blu4Net
{
    [XmlRoot("status")]
    public class Status
    {
        [XmlElement("volume")]
        public string VolumePercent;

        [XmlElement("db")]
        public string VolumeDB;

        [XmlElement("service")]
        public string Service;

        [XmlElement("title1")]
        public string Title1;

        [XmlElement("title2")]
        public string Title2;

        [XmlElement("title3")]
        public string Title3;
    }
}
