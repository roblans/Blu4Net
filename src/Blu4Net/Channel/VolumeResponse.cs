using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Xml.Serialization;

namespace Blu4Net.Channel
{
    [XmlRoot("volume")]
    public class VolumeResponse : LongPollingResponse
    {
        [XmlAttribute("db")]
        public double Decibel;

        [XmlAttribute("mute")]
        public int Mute;

#if NETSTANDARD2_0
        // workaround for: XmlSerializer works inconsistently with XmlTextAttribute on derived classes
        // https://github.com/dotnet/runtime/issues/22656
        [XmlText()]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public string VolumeInternal;
        public int Volume => int.Parse(VolumeInternal);
#else
        [XmlText()]
        public int Volume;
#endif

        public override string ToString()
        {
            return $"{Volume}% {Decibel}db";
        }
    }
}
