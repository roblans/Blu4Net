using Blu4Net.Channel;
using System;

namespace Blu4Net
{
    public class Volume
    {
        public double Decibel { get; }

        public bool IsMuted { get; }

        public int Percentage { get; }

        public Volume(VolumeResponse response)
        {
            if (response == null)
                throw new ArgumentNullException(nameof(response));

            Decibel = response.Decibel;
            IsMuted = response.Mute == 1;
            Percentage = response.Volume;
        }

        public override string ToString()
        {
            return $"{Percentage}% {Decibel}db";
        }
    }
}
