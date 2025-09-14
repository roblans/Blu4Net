using System.Xml.Serialization;

namespace Blu4Net.Channel
{
    [XmlRoot("status")]
    public class StatusResponse : ILongPollingResponse
    {
        [XmlElement("actions")]
        public ActionsArray Actions = new ActionsArray();

        [XmlAttribute("etag")]
        public string ETag { get; set; }

        [XmlElement("state")]
        public string State;

        [XmlElement("streamFormat")]
        public string StreamFormat;

        [XmlElement("quality")]
        public string Quality;

        [XmlElement("volume")]
        public int Volume;

        [XmlElement("canSeek")]
        public int CanSeek;

        [XmlElement("db")]
        public double Decibel;

        [XmlElement("image")]
        public string Image;

        [XmlElement("service")]
        public string Service;

        [XmlElement("artist")]
        public string Artist;

        [XmlElement("artistid")]
        public string ArtistID;

        [XmlElement("album")]
        public string Album;

        [XmlElement("albumid")]
        public string AlbumID;

        [XmlElement("title1")]
        public string Title1;

        [XmlElement("title2")]
        public string Title2;

        [XmlElement("title3")]
        public string Title3;

        [XmlElement("song")]
        public int? Song; // can be empty

        [XmlElement("songid")]
        public string SongID;

        [XmlElement("trackstationid")]
        public string TrackstationID;

        [XmlElement("totlen")]
        public double TotalLength;

        [XmlElement("secs")]
        public int Seconds;

        [XmlElement("shuffle")]
        public int Shuffle;

        [XmlElement("repeat")]
        public int Repeat;

        [XmlElement("pid")]
        public string PlaylistID;

        [XmlElement("prid")]
        public string PresetsID;

        [XmlElement("is_preset")]
        public string IsPreset;

        [XmlElement("preset_name")]
        public string PresetName;

        [XmlElement("streamUrl")]
        public string StreamUrl;

        [XmlElement("serviceIcon")]
        public string ServiceIcon;

        public override string ToString()
        {
            return State;
        }

        public class ActionsArray
        {
            [XmlElement("action")]
            public Action[] Items = new Action[0];
        }

        [XmlRoot("action")]
        public class Action
        {
            [XmlAttribute("icon")]
            public string Icon;

            [XmlAttribute("name")]
            public string Name;

            [XmlAttribute("notification")]
            public string Notification;

            [XmlAttribute("text")]
            public string Text;

            [XmlAttribute("url")]
            public string Url;

            public override string ToString()
            {
                return Name;
            }
        }
    }
}
