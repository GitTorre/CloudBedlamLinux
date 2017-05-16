using System;
using System.Xml.Serialization;

namespace CloudBedlam.Config
{
    public class ChaosBase
    {
        [XmlAttribute("RunOrder")]
        public int RunOrder { get; set; }

        [XmlElement("Duration")]
        public int DurationInSeconds { get; set; }

        [XmlIgnore]
        public TimeSpan Duration => new TimeSpan(0, 0, DurationInSeconds);
    }
}
