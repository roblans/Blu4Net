using Blu4Net.Channel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blu4Net
{
    public class PlayerMusicSource : PlayerMusicSourceItem
    {
        public PlayerMusicSource(BluChannel channel, string key, string text)
            : base(channel, key, text)
        {
        }
    }
}
