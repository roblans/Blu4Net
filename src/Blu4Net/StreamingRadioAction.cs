using Blu4Net.Channel;
using System;

namespace Blu4Net
{
    public sealed class StreamingRadioAction
    {
        public Uri IconUri;

        public PlayerAction Action;

        public string Notification;

        public string Text;

        public string Url;

        public StreamingRadioAction(StatusResponse.Action action, Uri endpoint)
        {
            IconUri = BluParser.ParseAbsoluteUri(action.Icon, endpoint);
            Action = BluParser.ParseAction(action.Name);
            Notification = action.Notification;
            Text = action.Text;
            Url = action.Url;
        }

        public override string ToString()
        {
            return Enum.GetName(typeof(PlayerAction), Action);
        }
    }
}
