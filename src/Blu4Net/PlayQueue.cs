using Blu4Net.Channel;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public IObservable<PlayQueueInfo> Changes { get; }

        internal PlayQueue(BluChannel channel, StatusResponse status)
        {
            _channel = channel ?? throw new ArgumentNullException(nameof(channel));

            Changes = _channel.StatusChanges
            .SkipWhile(response => response.PlaylistID == status.PlaylistID)
            .DistinctUntilChanged(response => response.PlaylistID)
            .SelectAsync(async _ => await GetInfo());
        }

        public async Task<PlayQueueInfo>  GetInfo()
        {
            var status = await _channel.GetPlaylistStatus();
            return new PlayQueueInfo(status.Name, status.Length);
        }

        public async IAsyncEnumerable<IReadOnlyCollection<PlayQueueSong>> GetSongs(int pageSize)
        {
            await foreach (var list in _channel.GetPlaylistPaged(pageSize))
            {
                yield return list.Songs.Select(element => new PlayQueueSong(element.Artist, element.Album, element.Title)).ToArray();
            }
        }
    }
}
