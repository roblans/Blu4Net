using Blu4Net.Channel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blu4Net
{
    public class MusicSource : MusicSourceEntry
    {
        public MusicSource(BluChannel channel, string browseKey)
            : base(channel, browseKey, null)
        {
        }
    }
}
