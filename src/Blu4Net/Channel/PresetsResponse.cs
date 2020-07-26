using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Blu4Net.Channel
{
    [XmlRoot("presets")]
    public class PresetsResponse
    {
        [XmlElement("preset")]
        public Preset[] Presets = new Preset[0];

        public override string ToString()
        {
            return Presets?.Length.ToString() ?? base.ToString();
        }
    }

    [XmlRoot("preset")]
    public class Preset
    {
        [XmlAttribute("name")]
        public string Name;

        [XmlAttribute("image")]
        public string Image;

        [XmlAttribute("url")]
        public string Url;

        [XmlAttribute("volume")]
        public int Volume = -1;

        [XmlAttribute("id")]
        public int ID;

        public override string ToString()
        {
            return ID.ToString();
        }
    }

}
