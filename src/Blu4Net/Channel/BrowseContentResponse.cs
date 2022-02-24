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

        [XmlAttribute("serviceIcon")]
        public string ServiceIcon;

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

            [XmlAttribute("text2")]
            public string Text2;

            [XmlAttribute("contextMenuKey")]
            public string ContextMenuKey;

            [XmlAttribute("playURL")]
            public string PlayURL;

            [XmlAttribute("autoplayURL")]
            public string AutoplayURL;

            [XmlAttribute("image")]
            public string Image;

            public override string ToString()
            {
                return Text;
            }
        }

        [XmlElement("category")]
        public Category[] Categories = new Category[0];

        [XmlRoot("category")]
        public class Category
        {
            [XmlAttribute("text")]
            public string Text;

            [XmlElement("item")]
            public Item[] Items = new Item[0];

            public override string ToString()
            {
                return Text;
            }
        }
    }
}
