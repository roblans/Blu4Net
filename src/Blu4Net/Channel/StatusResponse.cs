using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Blu4Net.Channel
{
    [XmlRoot("status")]
    public class StatusResponse : LongPollingResponse
    {
        [XmlElement("state")]
        public string State;

        [XmlElement("volume")]
        public int Volume;

        [XmlElement("db")]
        public double Decibel;

        [XmlElement("image")]
        public string Image;

        [XmlElement("service")]
        public string Service;

        [XmlElement("artist")]
        public string Artist;

        [XmlElement("album")]
        public string Album;

        [XmlElement("title1")]
        public string Title1;

        [XmlElement("title2")]
        public string Title2;

        [XmlElement("title3")]
        public string Title3;

        [XmlElement("totlen")]
        public int TrackLength;

        [XmlElement("secs")]
        public int TrackPosition;

        [XmlElement("is_preset")]
        public int PresetNumber;

        [XmlElement("preset_name")]
        public string PresetName;

        public override string ToString()
        {
            return State;
        }
    }
}
