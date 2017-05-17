﻿using System;
using System.Diagnostics;
using System.IO;
using NLog;

namespace CloudBedlam.Operations
{
    internal abstract class OperationBase : IDisposable
    {
        public bool CanRun { get; }
        public int RunOrderId { get; }
        public TimeSpan Duration { get; private set; }
        public string Name => GetType().Name;
        public bool IsProcessCreated { get; private set; }
        public Process Process { get; private set; }
        protected readonly DisposeService<OperationBase> DisposeService;
        private static ILogger _logger;

        protected OperationBase(bool canRun, TimeSpan duration, int runOrderId)
        {
            CanRun = canRun;
            Duration = duration;
            RunOrderId = runOrderId;
            _logger = Program.Logger;

            DisposeService = new DisposeService<OperationBase>(this, ps =>
            {
                if (Process == null) return;
                DestroyProcess();
            });
        }

        public void Dispose()
        {
            DisposeService.Dispose(true);
        }

        public void CreateProcess()
        {
            var processParams = CreateProcessParams();
            if (processParams == null)
            {
                var e = new Exception("Failed to create process parameters");
                Program.Logger.Error(e);
                IsProcessCreated = false;
                Process = null;
            }
            Process = CreateProcess(processParams);
            IsProcessCreated = true;
        }

        public void DestroyProcess()
        {
            Process?.Dispose();
            Process = null;
            IsProcessCreated = false;
        }

        protected class ProcessParams
        {
            public FileInfo File { get; }
            public string Arguments { get; }

            public ProcessParams(FileInfo file, string arguments)
            {
                File = file;
                Arguments = arguments;
            }
        }

        protected abstract ProcessParams CreateProcessParams();

        internal abstract void Kill();

        private Process CreateProcess(ProcessParams parameters)
        {
			var process = new Process();

			process.StartInfo.FileName = parameters.File.Name;
			process.StartInfo.UseShellExecute = false;
			process.StartInfo.CreateNoWindow = true;
			process.StartInfo.RedirectStandardError = true;
			process.StartInfo.RedirectStandardOutput = true;
			process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
			process.StartInfo.Arguments = !string.IsNullOrEmpty(parameters.Arguments) ? parameters.Arguments : string.Empty;
			process.EnableRaisingEvents = true;
      

            process.OutputDataReceived += (o, e) =>
            {
                _logger?.Debug($"{parameters.File?.Name}: {e?.Data}");
            };

            process.ErrorDataReceived += (o, e) => 
            {
                _logger?.Error(new Exception($"{parameters.File?.Name}: {e?.Data}"));
            };

			process.Exited += (o, e) =>
			{
				_logger?.Info($"Process {parameters.File?.Name} has exited.");
				DestroyProcess();
			};

            return process;
        }
    }
}