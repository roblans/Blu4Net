using Blu4Net.Channel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blu4Net
{
    public class Volume : IEquatable<Volume>
    {
        public int Percentage { get; }
        public double Decibel { get; }

        private Volume(int percentage, double decibel)
        {
            if (percentage < 0 || percentage > 100)
                throw new ArgumentOutOfRangeException(nameof(percentage), "Value must be between 0 and 100");
            if (decibel < -100 || decibel > 0)
                throw new ArgumentOutOfRangeException(nameof(decibel), "Value must be between -100 and 0");

            Percentage = percentage;
            Decibel = decibel;
        }

        internal static Volume FromResponse(VolumeResponse response)
        {
            return FromPercentage(response.Volume);
        }

        public static Volume FromPercentage(int percentage)
        {
            if (percentage < 0 || percentage > 100)
                throw new ArgumentOutOfRangeException(nameof(percentage), "Value must be between 0 and 100");

            var decibel = percentage != 0 ? Math.Round(45.0 * Math.Log10(percentage / 100.0), 1) : -100;
            decibel = Math.Max(-100, Math.Min(0, decibel));

            return new Volume(percentage, decibel);
        }

        public static Volume FromDecibel(double decibel)
        {
            if (decibel > 0)
                throw new ArgumentOutOfRangeException(nameof(decibel), "Value must be smaller or equal to zero");

            var percentage = decibel != -100 ? (int)Math.Round(Math.Pow(10, decibel / 45.0) * 100) : 0;
            percentage = Math.Max(0, Math.Min(100, percentage));

            return new Volume(percentage, decibel);
        }

        public override string ToString()
        {
            return $"{Percentage}% [{Decibel}db]";
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Volume);
        }

        public bool Equals(Volume other)
        {
            return other != null &&
                   Percentage == other.Percentage;
        }

        public override int GetHashCode()
        {
            return 355317789 + Percentage.GetHashCode();
        }

        public static bool operator ==(Volume left, Volume right)
        {
            return EqualityComparer<Volume>.Default.Equals(left, right);
        }

        public static bool operator !=(Volume left, Volume right)
        {
            return !(left == right);
        }
    }
}
