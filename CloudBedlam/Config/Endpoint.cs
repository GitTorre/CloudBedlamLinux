using System.Collections.Generic;

namespace CloudBedlam.Config
{
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