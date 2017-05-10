using System;

namespace CloudBedlam.Config
{
    [Serializable]
    public class ChaosOperation : ChaosBase
    {
        public int PressureLevel { get; set; }
    }
}
