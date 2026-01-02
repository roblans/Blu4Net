using System;
using System.Xml.Serialization;

namespace Blu4Net.Channel
{
    [XmlRoot("radiotime")]
    public class RadioBrowseResponse
    {
        [XmlAttribute("total_count")]
        public int TotalCount;

        [XmlElement("item")]
        public Item[] Items = new Item[0];

        public override string ToString()
        {
            return $"Items: {Items?.Length ?? 0}";
        }

        [XmlRoot("item")]
        public class Item
        {
            [XmlAttribute("id")]
            public string Id;
            
            [XmlAttribute("type")]
            public string Type;

            [XmlAttribute("text")]
            public string Text;

            [XmlAttribute("image")]
            public string Image;

            [XmlAttribute("URL")]
            public string UrlRaw ;
            
            [XmlIgnore]
            public string Url => Uri.UnescapeDataString(UrlRaw ?? string.Empty);

            [XmlAttribute("key")]
            public string Key;

            [XmlAttribute("is_active")]
            public int IsActive;

            public override string ToString()
            {
                return Text ?? Key ?? base.ToString();
            }
        }
    }
}

