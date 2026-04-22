using System.Xml.Serialization;

namespace Blu4Net.Channel
{
    [XmlRoot("error")]
    public class ErrorResponse
    {
        [XmlElement("message")]
        public string Message { get; set; }
    }
}