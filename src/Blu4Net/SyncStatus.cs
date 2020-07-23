using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Blu4Net
{
    [XmlRoot("SyncStatus")]
    public class SyncStatus
    {
        [XmlAttribute("volume")]
        public int Volume;
    }
}
