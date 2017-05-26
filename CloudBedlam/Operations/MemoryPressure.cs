using System;
using System.Diagnostics;
using System.IO;
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
			var filePath = "/usr/bin/bash";
			//--vm-bytes $(awk '/MemFree/{printf "%d\n", $2 * 0.097;}' < /proc/meminfo)k --vm-keep -m 10
			//--vm 4 --vm-bytes $(awk '/MemFree/{printf "%d\n", $2 * 0.097;}' < /proc/meminfo)k --mmap 2 --mmap-bytes 2G --page-in --timeout 10s
			Program.Logger.Info("--vm 4 --vm-bytes $(awk \'/MemFree/{printf \"%d\\n\", $2 * 0." + _config.PressureLevel + ";}\' < /proc/meminfo)k --mmap 2 --mmap-bytes 2G --page-in --timeout " + _config.DurationInSeconds + "s");
			//return new ProcessParams(new FileInfo(filePath), "--vm 4 --vm-bytes $(awk \'/MemFree/{printf \"%d\\n\", $2 * 0." + _config.PressureLevel + ";}\' < /proc/meminfo)k --mmap 2 --mmap-bytes 2G --page-in --timeout " + _config.DurationInSeconds + "s");
			return new ProcessParams(new FileInfo(filePath), "Bash/stress-mem.sh " + _config.PressureLevel + " " + _config.DurationInSeconds + "s");
		}
		//TODO: Play with sudo...
		// FileName = "/usr/bin/sudo";
		// Arguments = "/usr/bin/stress-ng etc...";
		private Tuple<string, string> RunMemoryStress(EmulationConfiguration config)
		{
			//TODO: mem allocation math...
			/*
			ct@ct-MacBookAir:~$ awk '/MemTotal/ || /SwapTotal/' /proc/meminfo
			MemTotal:        3950192 kB
			SwapTotal:       4097020 kB

			ct@ct-MacBookAir:~$ awk '/MemTotal/ || /MemAvailable/' /proc/meminfo
			MemTotal:        3950192 kB
			MemAvailable:    1955528 kB
			*/

			Process process = new Process();
			ProcessStartInfo processStartInfo = new ProcessStartInfo();
			processStartInfo.FileName = "stress-ng";
			processStartInfo.Arguments = "--vm 4 --vm-bytes $(awk '/MemFree/{printf \"%d\\n\", $2 * 0." + _config.PressureLevel + ";}' < /proc/meminfo)k --mmap 2 --mmap-bytes 2G --page-in --timeout " + _config.DurationInSeconds + "s";
			processStartInfo.RedirectStandardOutput = true;
			processStartInfo.RedirectStandardError = true;
			processStartInfo.UseShellExecute = false;
			processStartInfo.Verb = "RunAs";
			process.StartInfo = processStartInfo;

			process.Start();
			string error = process?.StandardError.ReadToEnd();
			string output = process?.StandardOutput.ReadToEnd();
			process.WaitForExit(_config.Duration.Milliseconds);


			var tuple = new Tuple<string, string>(error, output);

			return tuple;
	
		}

        internal override void Kill()
        {
            if (Process != null && Process.IsRunning() && !Process.HasExited)
            {
                Process?.Kill();
            }
        }
    }
}
