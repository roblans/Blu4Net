using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Blu4Net.Channel
{
    [XmlRoot("state")]
    public class PlayResponse
    {
        [XmlText()]
        public string State;
        public override string ToString()
        {
            return State;
        }
    }
}
