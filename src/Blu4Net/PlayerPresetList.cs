using Blu4Net.Channel;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reactive.Linq;
using System.Reactive;

namespace Blu4Net
{
    public class PlayerPresetList
    {
        private readonly BluChannel _channel;

        public IObservable<IReadOnlyCollection<PlayerPreset>> Changes { get; }

        public PlayerPresetList(BluChannel channel, StatusResponse status)
        {
            _channel = channel ?? throw new ArgumentNullException(nameof(channel));

            Changes = _channel.StatusChanges
            .SkipWhile(response => response.PresetsID == status.PresetsID)
            .DistinctUntilChanged(response => response.PresetsID)
            .SelectAsync(async _ => await GetPresets().ConfigureAwait(false));
        }


        public async Task<IReadOnlyCollection<PlayerPreset>> GetPresets()
        {
            var response = await _channel.GetPresets().ConfigureAwait(false);
            return response.Presets
                .Select(element => new PlayerPreset(element, _channel.Endpoint))
                .ToArray();
        }

        public Task LoadPreset(int number)
        {
            return _channel.LoadPreset(number);
        }
    }
}
