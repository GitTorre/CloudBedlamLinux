using System;
namespace CloudBedlam.Config
{
	public class ReorderConfiguration : EmulationConfiguration
	{
		public double PacketPercentage { get; set; } = 0.25;
		public double CorrelationPercentage { get; set; } = 0.50;
	}
}
