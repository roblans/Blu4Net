using System;
using System.Collections.Generic;
using System.Text;

namespace Blu4Net
{
    public class PlayQueueInfo
    {
        public string Name { get; private set; }
        public int Length { get; private set; }

        public PlayQueueInfo(string name, int length)
        {
            Name = name;
            Length = length;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
