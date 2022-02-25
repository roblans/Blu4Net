using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace Blu4Net.Channel
{
    public interface ILongPollingResponse
    {
        string ETag { get; set; }
    }
}
