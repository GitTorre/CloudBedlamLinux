using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CloudBedlam.Config
{
<<<<<<< HEAD
    [XmlRoot("ChaosConfiguration")]
    public class ChaosConfiguration
    {
        [XmlAttribute("Duration")]
        public int DurationInSeconds { get; set; }
        [XmlIgnore]
        public TimeSpan Duration => new TimeSpan(0, 0, DurationInSeconds);
        [XmlAttribute("Orchestration")]
        public Orchestration Orchestration { get; set; } = Orchestration.Unknown;
        [XmlAttribute("RunDelay")]
        public int RunDelay { get; set; } = 10;
        [XmlAttribute("Repeat")]
        public int Repeat { get; set; }
=======
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
>>>>>>> origin/master

		public ChaosOperation CpuPressure { get; set; } = null;
		public ChaosOperation MemoryPressure { get; set; } = null;
		public NetworkEmulation NetworkEmulation { get; set; } = null;
		public ChaosOperation DiskPressure { get; set; } = null;
	}
}