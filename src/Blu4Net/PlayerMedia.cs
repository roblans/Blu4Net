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
        public IReadOnlyList<string> Titles { get; private set; }
        public Uri ImageUri { get; private set; }
        public Uri ServiceIconUri { get; private set; }

        private PlayerMedia()
        {
        }

        public static PlayerMedia Create(StatusResponse response, Uri endpoint)
        {
            if (response == null)
                throw new ArgumentNullException(nameof(response));

            var imageUri = response.Image != null ? BluParser.ParseAbsoluteUri(response.Image, endpoint) : null;
            var serviceIconUri = response.ServiceIcon != null ? BluParser.ParseAbsoluteUri(response.ServiceIcon, endpoint) : null;
            var titles = new[] { response.Title1, response.Title2, response.Title3 }.Where(element => element != null).ToArray();

            return new PlayerMedia()
            {
                Titles = titles,
                ImageUri = imageUri,
                ServiceIconUri = serviceIconUri,
            };
        }

        public override string ToString()
        {
            return Titles.FirstOrDefault() ?? base.ToString();
        }
    }
}
