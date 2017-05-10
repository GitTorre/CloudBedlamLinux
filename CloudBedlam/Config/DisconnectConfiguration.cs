namespace CloudBedlam.Config
{
    public class DisconnectConfiguration : EmulationConfiguration
    {
        public double PeriodicDisconnectionRate { get; set; } = 0.5;
        public uint ConnectionTime { get; set; } = 5;
        public uint DisconnectionTime { get; set; } = 15;
    }
}
