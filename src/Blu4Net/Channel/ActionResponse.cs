using System.Xml.Serialization;

namespace Blu4Net.Channel
{
    public class ActionResponse
    {
    }

    [XmlRoot("response")]
    public class NotificationActionResponse : ActionResponse
    {
        [XmlText]
        public string Text;

        public override string ToString()
        {
            return $"Notification: {Text}";
        }
    }

    [XmlRoot("back")]
    public class BackActionResponse : ActionResponse
    {
        public override string ToString()
        {
            return "back";
        }
    }

    [XmlRoot("skip")]
    public class SkipActionResponse : ActionResponse
    {
        public override string ToString()
        {
            return "skip";
        }
    }

    [XmlRoot("ban")]
    public class BanActionResponse : ActionResponse
    {
        [XmlText]
        public string Text;

        public override string ToString()
        {
            return $"ban {Text}";
        }
    }

    [XmlRoot("love")]
    public class LoveActionResponse : ActionResponse
    {
        [XmlAttribute("skip")]
        public string Skip;

        [XmlText()]
        public string Text;

        public override string ToString()
        {
            return $"Love: {Text}";
        }
    }

    [XmlRoot("state")]
    public class StateActionResponse : ActionResponse
    {
        [XmlText()]
        public string Text;

        public override string ToString()
        {
            return $"State: {Text}";
        }
    }
}
