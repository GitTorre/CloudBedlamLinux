using System;
using Newtonsoft.Json;

namespace CloudBedlam.Config
{
<<<<<<< HEAD
    public class ChaosBase
    {
        [XmlAttribute("RunOrder")]
        public int RunOrder { get; set; }
=======
	public class ChaosBase
	{
		public int RunOrder { get; set; }
>>>>>>> origin/master

		[JsonProperty("Duration")]
		public int DurationInSeconds { get; set; }

		[JsonIgnore]
		public TimeSpan Duration => new TimeSpan(0, 0, DurationInSeconds);
	}
}