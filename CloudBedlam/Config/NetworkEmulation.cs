using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CloudBedlam.Config
{
    public class NetworkEmulation : ChaosBase
    {
        [JsonConverter(typeof(StringEnumConverter))]
		public NetworkEmProfile EmulationType { get; set; }
	    //-> Latency
        public uint LatencyDelay { get; set; }
		//-> Bandwidth
        public double BandwidthUpstreamSpeed { get; set; }
        public double BandwidthDownstreamSpeed { get; set; }
        public string ProtocolLayerType { get; set; } = "tcp";
        public string NetworkLayerType { get; set; } = "ipv4";
        /*** Loss (packet drop/decay) ***/
		[JsonConverter(typeof(StringEnumConverter))]
        public LossType LossType { get; set; }
        //-> random loss
        public double LossRate { get; set; }
        //-> burst loss
        public double BurstRate { get; set; }
        //-> Disconnect TODO...
        public double PeriodicDisconnectionRate { get; set; }
        public uint ConnectionTime { get; set; }
        public uint DisconnectionTime { get; set; }
		//-> Reorder/Corruption
		public double PacketPercentage { get; set; }
		public double CorrelationPercentage { get; set; }
	    //-> Endpoints
	    public TargetEndpoints TargetEndpoints { get; set; }

        // TODO: Move to NetworkProfile
        public bool IsValidProfile()
        {
            return EmulationType != NetworkEmProfile.Unknown;
        }
    }
}
