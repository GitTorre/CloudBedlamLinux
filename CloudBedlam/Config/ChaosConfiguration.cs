using System;
using System.Xml.Serialization;

namespace CloudBedlam.Config
{
    [Serializable]
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

        public ChaosOperation CpuPressure { get; set; } = null;
        public ChaosOperation MemoryPressure { get; set; } = null;
        public NetworkEmulation NetworkEmulation { get; set; } = null;
        public ChaosOperation DiskPressure { get; set; } = null;
    }
}