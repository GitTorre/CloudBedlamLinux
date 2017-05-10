namespace CloudBedlam.Config
{
    public class LatencyConfiguration : EmulationConfiguration
    {
        public uint FixedLatencyDelayMilliseconds { get; set; } = 250;
    }
}
