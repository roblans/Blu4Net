using Blu4Net.Channel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blu4Net
{
    public class PlayQueueInfo
    {
        public string Name { get; private set; }
        public int Length { get; private set; }

        public PlayQueueInfo(PlaylistStatusResponse response)
        {
            if (response == null)
                throw new ArgumentNullException(nameof(response));

            Name = response.Name;
            Length = response.Length;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
