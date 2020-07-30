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
        public IObservable<Unit> Changes { get; }

        internal PlayQueue(BluChannel channel, string currentPlayQueueID)
        {
            _channel = channel ?? throw new ArgumentNullException(nameof(channel));

            Changes = _channel.StatusChanges
            .Where(response => response.PlaylistID != currentPlayQueueID)
            .DistinctUntilChanged(response => response.PlaylistID)
            .Do(response => Console.WriteLine($"PlaylistID: {response.PlaylistID}"))
            .Select(response => Unit.Default);
        }

        public async Task<PlayQueueInfo>  GetInfo()
        {
            var status = await _channel.GetPlaylistStatus();
            return new PlayQueueInfo(status.Name, status.Length);
        }

        public async Task<IReadOnlyCollection<PlayQueueSong>> GetSongs()
        {
            var list = await _channel.GetPlaylist();
            return list.Songs.Select(element => new PlayQueueSong(element.Artist, element.Album, element.Title)).ToArray();
        }

        public async IAsyncEnumerable<IReadOnlyCollection<PlayQueueSong>> GetSongsPaged(int pageSize)
        {
            await foreach (var list in _channel.GetPlaylistPaged(pageSize))
            {
                yield return list.Songs.Select(element => new PlayQueueSong(element.Artist, element.Album, element.Title)).ToArray();
            }
        }
    }
}
