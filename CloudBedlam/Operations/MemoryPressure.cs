using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using CloudBedlam.Config;
using CloudBedlam.Extensions;

namespace CloudBedlam.Operations
{
	internal class MemoryPressure : OperationBase
	{
		private readonly ChaosOperation _config;

		public MemoryPressure(ChaosOperation config, TimeSpan testDuration)
			: base(config.PressureLevel > 0,
				  config.Duration > testDuration ? testDuration : config.Duration,
				  config.RunOrder)
		{
			_config = config;
		}

		protected override ProcessParams CreateProcessParams()
		{
			const string filePath = "/usr/bin/bash";
			return new ProcessParams(new FileInfo(filePath), "Bash/stress-mem.sh " + _config.PressureLevel + " " + _config.DurationInSeconds);
		}


        internal override void Kill()
        {
	        if (Process == null || !Process.IsRunning() || Process.HasExited) 
		        return;
	        
	        Thread.Sleep(4000);
	        Process?.Kill();
        }
    }
}
