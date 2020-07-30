using Blu4Net.Channel;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blu4Net
{
    public class PlayQueue
    {
        private readonly BluChannel _channel;

        private IDisposable _subscription;

        public string Name { get; private set; }
        public int Length { get; private set; }

        internal PlayQueue(BluChannel channel)
        {
            _channel = channel ?? throw new ArgumentNullException(nameof(channel));
        }

        internal async Task Connect()
        {
            var status = await _channel.GetPlaylistStatus();
            Name = status.Name;
            Length = status.Length;

            _subscription = _channel.StatusChanges
                .DistinctUntilChanged(response => response.PlayQueueID)
                .Subscribe(response => OnQueueChanged(response.PlayQueueID));
        }

        internal ValueTask Disconnect()
        {
            _subscription.Dispose();
            return default;
        }

        private void OnQueueChanged(string id)
        {
            Console.WriteLine($"Queue changed: {id}");
        }
    }
}
