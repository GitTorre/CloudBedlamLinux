using System;
using System.IO;
using System.Threading;
using Newtonsoft.Json;
using CloudBedlam.Config;
using NLog;

namespace CloudBedlam
{
    internal class Program
    {
        //  Chaos configuration settings...
        private static ChaosConfiguration _chaosConfiguration;
        private static int _repeat;
        private static int _runDelay;
        //Logging...
        public static ILogger Logger;
        //Bedlam
        private static Bedlam _bedlam;

        private static void Main()
        {
            Logger = LogManager.GetLogger(nameof(Program));

            var configPath = Path.Combine(Environment.CurrentDirectory, "Chaos.json");

            if (File.Exists(configPath))
            {
                _chaosConfiguration = GetChaosConfiguration(configPath);
            }

            if (_chaosConfiguration == null)
            {
                // if GetChaosConfiguration fails (which it logs...), then exit...
                Logger?.Info("No configruration supplied... Exiting...");
                Environment.Exit(-1);
            }

            //Repeat setting
            if (_chaosConfiguration.Repeat > 0)
            {
                _repeat = _chaosConfiguration.Repeat;
            }

            //Start delay setting
            if (_chaosConfiguration.RunDelay > 0)
            {
                _runDelay = _chaosConfiguration.RunDelay;
            }

            try
            {
                //Must initialize Bedlam instance with ChaosConfiguration instance...
                _bedlam = new Bedlam(_chaosConfiguration);
            }
            catch (Exception e) 
            {
                Logger?.Info("Error initializing Bedlam: " + e.Message + "\n Exiting...");
                Environment.Exit(-1);
            }

            for (int i = 0; i < _repeat + 1; i++)
            {
                if (_runDelay > 0) Thread.Sleep(_runDelay * 1000);
                _bedlam.Run();
            }

        }
        /// <summary>
        /// Returns a ChaosConfiguaration object with properties set from supplied config file
        /// throws an Exception if anything goes wrong, so try-catch the call to this and handle it...
        /// </summary>
        /// <param name="configPath">full path to Chaos.config file</param>
        /// <returns>ChaosConfguration</returns>
        private static ChaosConfiguration GetChaosConfiguration(string configPath)
		{
			if (!File.Exists(configPath)) return null;

			try
			{
				var config = File.ReadAllText(configPath);
				_chaosConfiguration = JsonConvert.DeserializeObject<ChaosConfiguration>(config);
				return _chaosConfiguration;
			}
			catch (Exception e)
			{
				Logger?.Error(e);
				return null;
			}
		}
   }
}
 