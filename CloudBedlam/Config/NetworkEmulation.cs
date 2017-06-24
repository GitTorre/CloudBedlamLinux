using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CloudBedlam.Config
{
    public class NetworkEmulation : ChaosBase
    {
        [JsonConverter(typeof(StringEnumConverter))]
		public NetworkEmProfile EmulationType { get; set; }
        public uint LatencyDelay { get; set; }
        public TargetEndpoints TargetEndpoints { get; set; }
		//-> Bandwidth
        public double BandwidthUpstreamSpeed { get; set; }
        public double BandwidthDownstreamSpeed { get; set; }
        public string ProtocolLayerType { get; set; } = "tcp";
        public string NetworkLayerType { get; set; } = "ipv4";
        /*** Loss (packet drop/decay) ***/
		[JsonConverter(typeof(StringEnumConverter))]
        public LossType LossType { get; set; }
        //-> periodic loss
        public uint PeriodicLossPeriod { get; set; } = 10;
        //-> random loss
        public double RandomLossRate { get; set; } = 0.5;
        //-> burst loss
        public double BurstRate { get; set; } = 0.5;
        public uint MinimumBurst { get; } = 1;
        public uint MaximumBurst { get; } = 20;
        //-> Disconnect
        public double PeriodicDisconnectionRate { get; set; } = 0.5;
        public uint ConnectionTime { get; set; } = 5;
        public uint DisconnectionTime { get; set; } = 15;
		//-> Reorder/Corruption
		public double PacketPercentage { get; set; } = 0.25;
		public double CorrelationPercentage { get; set; } = 0.50;

        // TODO: Move to NetworkProfile
        public bool IsValidProfile()
        {
            return EmulationType != NetworkEmProfile.Unknown;
        }
    }
}
