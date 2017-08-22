using System;
using Newtonsoft.Json;

namespace CloudBedlam.Config
{
	public class ChaosBase
	{
		public int RunOrder { get; set; }

		[JsonProperty("Duration")]
		public int DurationInSeconds { get; set; }

		[JsonIgnore]
		public TimeSpan Duration => new TimeSpan(0, 0, DurationInSeconds);
	}
}