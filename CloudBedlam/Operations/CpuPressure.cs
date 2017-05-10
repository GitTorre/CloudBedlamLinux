using System;
using System.Diagnostics;
using System.IO;
using CloudBedlam.Config;
using CloudBedlam.Extensions;
//For Linux resource stressing: stress-ng is the tool to use for Ubuntu, etc...
//Installation: sudo apt install stress-ng
/*
 EXAMPLES:
 
       stress-ng --cpu 4 --io 2 --vm 1 --vm-bytes 1G --timeout 60s

              runs  for  60 seconds with 4 cpu stressors, 2 io stressors and 1
              vm stressor using 1GB of virtual memory.

       stress-ng --cpu 8 --cpu-ops 800000

              runs 8 cpu stressors and stops after 800000 bogo operations.

       stress-ng --sequential 2 --timeout 2m --metrics

              run 2 simultaneous instances of all the  stressors  sequentially
              one  by  one,  each for 2 minutes and summarise with performance
              metrics at the end.

       stress-ng --cpu 4 --cpu-method fft --cpu-ops 10000 --metrics-brief

              run 4 FFT cpu stressors, stop after 10000  bogo  operations  and
              produce a summary just for the FFT results.

       stress-ng --cpu 0 --cpu-method all -t 1h

              run  cpu  stressors  on  all online CPUs working through all the
              available CPU stressors for 1 hour.

       stress-ng --all 4 --timeout 5m

              run 4 instances of all the stressors for 5 minutes.

       stress-ng --random 64

              run 64 stressors that are randomly chosen from all the available
              stressors.

       stress-ng --cpu 64 --cpu-method all --verify -t 10m --metrics-brief

              run  64  instances of all the different cpu stressors and verify
              that the computations are correct for 10  minutes  with  a  bogo
              operations summary at the end.

       stress-ng --sequential 0 -t 10m

              run all the stressors one by one for 10 minutes, with the number
              of instances of each stressor  matching  the  number  of  online
              CPUs.

       stress-ng --sequential 8 --class io -t 5m --times

              run  all  the stressors in the io class one by one for 5 minutes
              each, with 8 instances of each stressor running concurrently and
              show overall time utilisation statistics at the end of the run.

       stress-ng --all 0 --maximize --aggressive

              run   all   the   stressors   (1   instance  of  each  per  CPU)
              simultaneously,  maximize  the  settings  (memory  sizes,   file
              allocations,  etc.)  and  select  the  most demanding/aggressive
              options.

       stress-ng --random 32 -x numa,hdd,key

              run 32 randomly selected stressors and exclude the numa, hdd and
              key stressors

       stress-ng --sequential 4 --class vm --exclude bigheap,brk,stack

              run  4  instances  of  the  VM  stressors  one after each other,
              excluding the bigheap, brk and stack stressors
*/
namespace CloudBedlam.Operations
{
    internal class CpuPressure : OperationBase
    {
        private readonly ChaosOperation _config;

        public CpuPressure(ChaosOperation config, TimeSpan testDuration) 
            : base(config.PressureLevel > 0 && config.DurationInSeconds > 0,
                  config.Duration > testDuration ? testDuration : config.Duration,
                  config.RunOrder)
        {
            _config = config;
        }

        protected override ProcessParams CreateProcessParams()
        {
			var filePath = "/bin/bash";
			return new ProcessParams(new FileInfo(filePath), "-c 'sudo stress-ng --cpu 0 --cpu-method all -t " + _config.DurationInSeconds + "s'");
        }

		//TODO: Play with sudo...
		private Tuple<string, string> RunCpuStress(EmulationConfiguration config)
		{
			Process process = new Process();
			ProcessStartInfo processStartInfo = new ProcessStartInfo();
			processStartInfo.FileName = "/bin/bash";
			processStartInfo.WorkingDirectory = "/home";
			processStartInfo.Arguments = "sudo stress-ng --cpu 0 --cpu-method all -t " + _config.DurationInSeconds + "s";
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
