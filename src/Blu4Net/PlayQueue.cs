using Blu4Net.Channel;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blu4Net
{
    public class PlayQueue
    {
        private readonly BluChannel _channel;
        public IObservable<Unit> Changes { get; }

        internal PlayQueue(BluChannel channel)
        {
            _channel = channel ?? throw new ArgumentNullException(nameof(channel));

            Changes = _channel.StatusChanges
            .DistinctUntilChanged(response => response.PlayQueueID)
            .Select(response => Unit.Default);
        }

        public async Task<PlayQueueInfo>  GetInfo()
        {
            var status = await _channel.GetPlaylistStatus();
            return new PlayQueueInfo(status.Name, status.Length);
        }
    }
}
