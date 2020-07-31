using System;
using System.Collections.Generic;
using System.Text;

namespace Blu4Net
{
    public class PlayPosition
    {
        public TimeSpan CurrentValue { get; }
        public TimeSpan? TotalLength { get; }

        public PlayPosition(TimeSpan currentValue, TimeSpan? totalLength)
        {
            CurrentValue = currentValue;
            TotalLength = totalLength;
        }

        public override string ToString()
        {
            return TotalLength != null ? $"{CurrentValue} - {TotalLength}" : $"{CurrentValue}";
        }
    }
}
