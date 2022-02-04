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
        public IReadOnlyList<string> Titles { get; }
        public Uri ImageUri { get; }
        public Uri ServiceIconUri { get; }
        public string ServiceName { get; }
        public string Quality { get; }
        public string StreamFormat { get; }
        public PlayerState PlayerState { get; }
        public PlayerMedia(StatusResponse response, Uri endpoint)
        {
            if (response == null)
                throw new ArgumentNullException(nameof(response));

            ImageUri = response.Image != null ? BluParser.ParseAbsoluteUri(response.Image, endpoint) : null;
            ServiceIconUri = response.ServiceIcon != null ? BluParser.ParseAbsoluteUri(response.ServiceIcon, endpoint) : null;
            Titles = new[] { response.Title1, response.Title2, response.Title3 }.Where(element => element != null).ToArray();
            ServiceName = response.Service;
            Quality = response.Quality;
            StreamFormat = response.StreamFormat;
            PlayerState = BluParser.ParseState(response.State);
        }


        public override string ToString()
        {
            return Titles.FirstOrDefault() ?? base.ToString();
        }
    }
}
