namespace CloudBedlam.Config
{
    public class LossConfiguration : EmulationConfiguration
    {
        // Burst loss: packet loss occurs according to a given probability, and when a packet loss occurs, multiple packets losses occur consecutively.
        // Periodic loss: packet loss occurs periodically, that is, one packet is dropped per given number of packets. 
        // Random loss: packet loss occurs randomly in a given loss rate.
        public LossType LossType { get; set; }
        //periodic loss
        public uint PeriodicLossPeriod { get; set; } = 10;
        //random loss
        public double RandomLossRate { get; set; } = 0.5;
        //burst loss
        public double BurstRate { get; set; } = 0.5;
        public uint MinimumBurst { get; set; } = 1;
        public uint MaximumBurst { get; set; } = 20;
    }

    public enum LossType
    {
        Burst,
        Periodic,
        Random
    }
}
