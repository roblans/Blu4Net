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

        internal PlayerPresetList(BluChannel channel, StatusResponse status)
        {
            _channel = channel ?? throw new ArgumentNullException(nameof(channel));

            Changes = _channel.StatusChanges
            .SkipWhile(response => response.PresetsID == status.PresetsID)
            .DistinctUntilChanged(response => response.PresetsID)
            .SelectAsync(async _ => await GetPresets());
        }

        public async Task<IReadOnlyCollection<PlayerPreset>> GetPresets()
        {
            var response = await _channel.GetPresets();
            return response.Presets
                .Select(element => new PlayerPreset(element.ID, element.Name, element.Image.ToAbsoluteUri(_channel.Endpoint)))
                .ToArray();
        }
    }
}
