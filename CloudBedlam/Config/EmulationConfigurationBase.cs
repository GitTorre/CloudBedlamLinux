namespace CloudBedlam.Config
{
    public class EmulationConfiguration
    {
        public TargetEndpoints TargetEndpoints { get; set; }
        public string ProtocolLayerType { get; set; } = "tcp";
        public string NetworkLayerType { get; set; } = "ipv4";
    }
}
