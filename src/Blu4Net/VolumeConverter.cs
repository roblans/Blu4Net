using System;
using System.Collections.Generic;
using System.Text;

namespace Blu4Net
{
    public static class VolumeConverter
    {
        public static double PercentageToDecibel(int percentage)
        {
            if (percentage < 0 || percentage > 100)
                throw new ArgumentOutOfRangeException(nameof(percentage), "Value must be between 0 and 100, inclusive.");

            return percentage > 0 ? Math.Round(45 * Math.Log10(percentage) - 90, 1) : -100;
        }

        public static int DecibelToPercentage(double decibel)
        {
            if (decibel > 0)
                throw new ArgumentOutOfRangeException(nameof(decibel), "Value must be less than or equal to 0.");

            return decibel > -100 ? (int)Math.Round(Math.Pow(10, (decibel + 90) / 45)) : 0;
        }
    }
}
