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
    public class PresetList
    {
        private readonly BluChannel _channel;

        public IObservable<Unit> Changes { get; }

        internal PresetList(BluChannel channel, string currentPrestListID)
        {
            _channel = channel ?? throw new ArgumentNullException(nameof(channel));

            Changes = _channel.StatusChanges
            .Where(response => response.PresetsID != currentPrestListID)
            .DistinctUntilChanged(response => response.PresetsID)
            .Select(response => Unit.Default);
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
