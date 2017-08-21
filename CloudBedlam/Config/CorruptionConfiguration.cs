using System;
namespace CloudBedlam.Config
{
	public class CorruptionConfiguration : EmulationConfiguration
	{
		public double PacketPercentage { get; set; } = 0.05;
	}
}
