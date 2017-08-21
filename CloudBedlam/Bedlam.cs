using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CloudBedlam.Config;
using CloudBedlam.Extensions;
using CloudBedlam.Operations;
using NLog;

namespace CloudBedlam
{
    public class Bedlam
    {
        readonly ChaosConfiguration _config;
        readonly ILogger _logger;
        readonly List<OperationBase> _operations = new List<OperationBase>();
        readonly Dictionary<Orchestration, Action> _runModes;

        internal Bedlam(ChaosConfiguration configuration)
        {
            _logger = Program.Logger;

            if (configuration == null)
            {
                _logger?.Error("Can't initialize Bedlam with chaos configuration settings. Check your Chaos.config file.");
                Environment.Exit(-1);
            }

            _config = configuration;

			//Check for null to support the use of one or more chaos operations in the config file... -CT
			if (_config.CpuPressure != null) _operations.Add(new CpuPressure(_config.CpuPressure, _config.Duration));
            if (_config.MemoryPressure != null) _operations.Add(new MemoryPressure(_config.MemoryPressure, _config.Duration));
            if (_config.NetworkEmulation != null) _operations.Add(new Operations.NetworkEmulation(_config.NetworkEmulation, _config.Duration));

            if (_operations.Count == 0)
            {
                _logger?.Error("No operations settings provided. Aborting....");
                throw new Exception(
                    "Cannot execute Bedlam without an operations setting. Check your Chaos.config file...");
            }
            
            if (_operations.Count == 1)
            {
                _config.Orchestration = Orchestration.Sequential;
            }

            if (_config.Orchestration == Orchestration.Unknown)
            {
                _logger?.Error("No orchestration setting provided. Aborting.");
                throw new Exception("Cannot execute Bedlam without an Orchestration setting. Check your Chaos.config file.");
            }

            _runModes = new Dictionary<Orchestration, Action>
            {
                { Orchestration.Concurrent, RunConcurrent },
                { Orchestration.Random, RunRandom },
                { Orchestration.Sequential, RunSequential }
            };
        }

        public void Run() 
        {
            try
            {
                Console.WriteLine("Starting Bedlam...");
                _runModes[_config.Orchestration]();
            }
            catch (Exception e)
            {
                _logger?.Error(e);
                Environment.Exit(-1);
            }
        }

        bool RunOperation(OperationBase operation)
        {
            if (operation == null) return false;

            try
            {
                if (!operation.IsProcessCreated)
                {
                    operation.CreateProcess();
                }
            
				_logger?.Info($"Starting {operation.Name}.");
                var  isStarted = operation.Process?.Start();
          

                var b = !isStarted;
                if (b != null && (bool) b)
                {
                    _logger?.Error($"The operation {operation.Name} did not start successfully.");
                    Kill(operation);
                    return false;
                }

                if (operation.Process != null)
                {
                    operation.Process?.BeginOutputReadLine();
                    operation.Process?.BeginErrorReadLine();
                }
            }
            catch (Exception e)
            {
                _logger?.Error(e);
                return false;
				//throwing here will kill the app, just move on to next operation, error is logged...
            }
            return true;
        }

        void Kill(OperationBase operation)
        {
            if (!operation.IsProcessCreated || !operation.Process.IsRunning() || operation.Process.HasExited)
            {
                return;
            }

            _logger?.Info($"Killing {operation.Name}.");
            operation.Kill();
        }

        void RunConcurrent()
        {
            if (_config.DurationInSeconds <= 0)
            {
                return;
            }
           
            foreach (var operation in _operations)
            {
                RunOperation(operation);
            }
            //Wait... We override individual durations for concurrent run for now...
            Thread.Sleep((int)_config.Duration.TotalMilliseconds);
            //Clean up...
            foreach (var operation in _operations)
            {
                operation.Process.CancelOutputRead();
                operation.Process.CancelErrorRead();
                Kill(operation);
            }
        }

        void RunSequential()
        {
            var operations = _operations.OrderBy(o => o.RunOrderId).DefaultIfEmpty();
            RunSequential(operations);
        }

        void RunSequential(IEnumerable<OperationBase> operations)
        {
            foreach (var operation in operations)
            {
                var isStarted = RunOperation(operation);
                
                if (!isStarted) continue;

                _logger.Info("Starting Sequential Run for " + operation.Name);
                operation.Process.WaitForExit((int)operation.Duration.TotalMilliseconds);
                operation.Process.CancelOutputRead();
                operation.Process.CancelErrorRead();
                _logger.Info("Ending Sequential run for " + operation.Name);
                Kill(operation);
            }
        }

        void RunRandom()
        {
            //http://stackoverflow.com/a/4651405/294804
            var operations = _operations.OrderBy(o => Guid.NewGuid());
            RunSequential(operations);
        }
    }
}
