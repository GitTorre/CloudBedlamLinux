using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CloudBedlam.Config
{
	public class ChaosConfiguration
	{
		[JsonProperty("Duration")]
		public int DurationInSeconds { get; set; }
		[JsonIgnore]
		public TimeSpan Duration => new TimeSpan(0, 0, DurationInSeconds);
		[JsonConverter(typeof(StringEnumConverter))]
		public Orchestration Orchestration { get; set; } = Orchestration.Unknown;
		public int RunDelay { get; set; } = 10;
		public int Repeat { get; set; }

		public ChaosOperation CpuPressure { get; set; } = null;
		public ChaosOperation MemoryPressure { get; set; } = null;
		public NetworkEmulation NetworkEmulation { get; set; } = null;
		public ChaosOperation DiskPressure { get; set; } = null;
	}
}