using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Blu4Net.Channel
{
    [XmlRoot("browse")]
    public class BrowseContentResponse
    {
        [XmlAttribute("serviceName")]
        public string ServiceName;

        [XmlAttribute("searchKey")]
        public string SearchKey;

        [XmlElement("item")]
        public Item[] Items = new Item[0];

        [XmlRoot("item")]
        public class Item
        {
            [XmlAttribute("browseKey")]
            public string BrowseKey;

            [XmlAttribute("type")]
            public string Type;

            [XmlAttribute("text")]
            public string Text;

            [XmlAttribute("image")]
            public string Image;

            public override string ToString()
            {
                return Text;
            }
        }
    }
}
