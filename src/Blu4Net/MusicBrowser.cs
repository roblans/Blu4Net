using Blu4Net.Channel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blu4Net
{
    public class MusicBrowser : MusicContentNode
    {
        public MusicBrowser(BluChannel channel, BrowseContentResponse response)
            : base(channel, null, response)
        {
        }
    }
}
