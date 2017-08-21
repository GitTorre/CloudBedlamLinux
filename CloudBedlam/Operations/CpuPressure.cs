using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using CloudBedlam.Config;
using CloudBedlam.Extensions;

namespace CloudBedlam.Operations
{
    class CpuPressure : OperationBase
    {
        readonly ChaosOperation _config;

        public CpuPressure(ChaosOperation config, TimeSpan testDuration)
            : base(config.PressureLevel > 0 && config.DurationInSeconds > 0,
                  config.Duration > testDuration ? testDuration : config.Duration,
                  config.RunOrder)
        {
            _config = config;
        }

        protected override ProcessParams CreateProcessParams()
        {
<<<<<<< HEAD
            var filePath = "/usr/bin/bash";
            return new ProcessParams(new FileInfo(filePath), "./Bash/stress-cpu.sh " + _config.PressureLevel + " " + _config.DurationInSeconds + "s");
        }

        //TODO: Play with sudo...
        private Tuple<string, string> RunCpuStress(EmulationConfiguration config)
        {
            Process process = new Process();
            ProcessStartInfo processStartInfo = new ProcessStartInfo();
            processStartInfo.FileName = "stress-ng";
            processStartInfo.Arguments = "--cpu 0 --cpu-method all -t " + _config.DurationInSeconds + "s";
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.RedirectStandardError = true;
            processStartInfo.UseShellExecute = false;
            //processStartInfo.Verb = "RunAs";
            process.StartInfo = processStartInfo;

            process.Start();
            string error = process?.StandardError.ReadToEnd();
            string output = process?.StandardOutput.ReadToEnd();
            process.WaitForExit(_config.Duration.Milliseconds);

            var tuple = new Tuple<string, string>(error, output);

            return tuple;

        }

=======
            const string filePath = "/usr/bin/bash";
            return new ProcessParams(new FileInfo(filePath), "Bash/stress-cpu.sh " + _config.PressureLevel + " " + _config.DurationInSeconds + "s");
        }

>>>>>>> origin/master

        internal override void Kill()
        {
            if (Process == null || !Process.IsRunning() || Process.HasExited) 
                return;
            
            Thread.Sleep(4000);
            Process?.Kill();
        }
    }
}
