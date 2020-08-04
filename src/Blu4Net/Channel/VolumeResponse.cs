﻿using System;
using System.Collections.Generic;
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
        [XmlText(typeof(string))]
        public string Volume;
#else
        [XmlText(typeof(int))]
        public int Volume;
#endif

        public override string ToString()
        {
            return $"{Volume}% {Decibel}db";
        }
    }
}
