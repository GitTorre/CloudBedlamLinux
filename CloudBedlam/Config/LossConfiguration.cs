namespace CloudBedlam.Config
{
    public class LossConfiguration : EmulationConfiguration
    {
        // Burst loss: packet loss occurs according to a given probability, and when a packet loss occurs,
        // multiple packets losses occur consecutively.
        // Random loss: packet loss occurs randomly in a given loss rate.
        public LossType LossType { get; set; }
        //random loss
        public double LossRate { get; set; }
        //burst loss 
        public double BurstRate { get; set; }
    }

    public enum LossType
    {
        Burst,
        Random
    }
}
