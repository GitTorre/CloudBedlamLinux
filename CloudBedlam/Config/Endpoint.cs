using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace CloudBedlam.Config
{

    [Serializable]
    public class TargetEndpoints
    {
        [XmlElement("Endpoint")]
        public List<Endpoint> Endpoints { get; set; }
    }

    [Serializable]
    public class Endpoint
    {
        [XmlAttribute("Port")]
        public string Port { get; set; }
        [XmlAttribute("Uri")]
        public string Uri { get; set; }
    }
}
