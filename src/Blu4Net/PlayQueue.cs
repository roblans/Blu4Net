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

        public PlayQueue(BluChannel channel, StatusResponse status)
        {
            _channel = channel ?? throw new ArgumentNullException(nameof(channel));

            Changes = _channel.StatusChanges
            .SkipWhile(response => response.PlaylistID == status.PlaylistID)
            .DistinctUntilChanged(response => response.PlaylistID)
            .SelectAsync(async _ => await GetInfo().ConfigureAwait(false));
        }

        public async Task<PlayQueueInfo>  GetInfo()
        {
            var status = await _channel.GetPlaylistStatus().ConfigureAwait(false);
            return new PlayQueueInfo(status);
        }

        public async IAsyncEnumerable<IReadOnlyCollection<PlayQueueSong>> GetSongs(int pageSize)
        {
            await foreach (var list in _channel.GetPlaylistPaged(pageSize).ConfigureAwait(false))
            {
                yield return list.Songs.Select(element => new PlayQueueSong(element)).ToArray();
            }
        }

        public Task Clear()
        {
            return _channel.Clear();
        }

        public Task Remove(int songID)
        {
            return _channel.Delete(songID);
        }

        public async Task<int> Save(string name)
        {
            var response = await _channel.Save(name).ConfigureAwait(false);
            return response.Entries;
        }
    }
}
