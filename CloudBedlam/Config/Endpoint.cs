using System.Collections.Generic;

namespace CloudBedlam.Config
{
<<<<<<< HEAD
    public class TargetEndpoints
    {
        [XmlElement("Endpoint")]
        public List<Endpoint> Endpoints { get; set; }
    }

    public class Endpoint
    {
        [XmlAttribute("Port")]
        public string Port { get; set; }
        [XmlAttribute("Uri")]
        public string Uri { get; set; }
    }
}
=======
	public class TargetEndpoints
	{
		public List<Endpoint> Endpoints { get; set; }
	}

	public class Endpoint
	{
		public int Port { get; set; }
		public string Protocol { get; set; }
		public string Hostname { get; set; }
	}
}
>>>>>>> origin/master
