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
    public class PlayerMedia
    {
        public IReadOnlyList<StreamingRadioAction> Actions { get; }
        public IReadOnlyList<string> Titles { get; }
        public Uri ImageUri { get; }
        public Uri ServiceIconUri { get; }
        public string ServiceName { get; }
        public string Quality { get; }
        public string StreamFormat { get; }
        public bool CanSeek { get; }
        public PlayerState PlayerState { get; }

        /// <summary>
        /// NAD: The position of the current track in the play queue. Also see streamUrl.
        /// </summary>
        public int? Song { get; }
        public PlayerMedia(StatusResponse response, Uri endpoint)
        {
            if (response == null)
                throw new ArgumentNullException(nameof(response));

            Actions = response.Actions.Items.Select(action => new StreamingRadioAction(action, endpoint)).ToArray();
            ImageUri = response.Image != null ? BluParser.ParseAbsoluteUri(response.Image, endpoint) : null;
            ServiceIconUri = response.ServiceIcon != null ? BluParser.ParseAbsoluteUri(response.ServiceIcon, endpoint) : null;
            Titles = new[] { response.Title1, response.Title2, response.Title3 }.Where(element => element != null).ToArray();
            ServiceName = response.Service;
            Quality = response.Quality;
            StreamFormat = response.StreamFormat;
            Song = response.Song;
            PlayerState = BluParser.ParseState(response.State);
            CanSeek = response.CanSeek == 1;
        }


        public override string ToString()
        {
            return Titles.FirstOrDefault() ?? base.ToString();
        }
    }
}
