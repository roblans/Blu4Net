using System;
using System.Collections.Generic;
using System.Text;

namespace Blu4Net
{
    public class PlayPosition
    {
        public TimeSpan Elapsed { get; }
        public TimeSpan? Length { get; }

        public PlayPosition(TimeSpan elapsed, TimeSpan? totalLength)
        {
            Elapsed = elapsed;
            Length = totalLength;
        }

        public override string ToString()
        {
            return Length != null ? $"{Elapsed} - {Length}" : $"{Elapsed}";
        }
    }
}
