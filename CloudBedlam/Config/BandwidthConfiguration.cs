namespace CloudBedlam.Config
{
    public class BandwidthConfiguration : EmulationConfiguration
    {
        public double DownstreamBandwidth { get; set; } = 33.6 * 1024;
        public double UpstreamBandwidth { get; set; } = 56 * 1024;
    }
}
