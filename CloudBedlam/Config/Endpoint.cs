using System.Collections.Generic;

namespace CloudBedlam.Config
{

	public class TargetEndpoints
	{
		public List<Endpoint> Endpoints { get; set; }
	}

	public class Endpoint
	{
		public string Port { get; set; }
		public string Uri { get; set; }
	}
}