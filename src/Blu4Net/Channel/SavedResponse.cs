using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Blu4Net.Channel
{
    [XmlRoot("saved")]
    public class SavedResponse
    {
        [XmlElement("entries")]
        public int Entries;

        public override string ToString()
        {
            return Entries.ToString();
        }
    }
}
