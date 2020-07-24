using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Blu4Net.Channel
{
    [XmlRoot("id")]
    public class SkipResponse
    {
        [XmlText()]
        public int ID;

        public override string ToString()
        {
            return ID.ToString();
        }
    }
}
