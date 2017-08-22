using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Linq;
using System.Net.Sockets;
using CloudBedlam.Config;
using CloudBedlam.Extensions;

namespace CloudBedlam.Operations
{
	class NetworkEmulation : OperationBase
	{
		readonly Config.NetworkEmulation _config;

		public NetworkEmulation(Config.NetworkEmulation config, TimeSpan testDuration)
			: base(config.IsValidProfile() && config.DurationInSeconds > 0,
				  config.Duration > testDuration ? testDuration : config.Duration,
				  config.RunOrder)
		{
			_config = config;
		}

		protected override ProcessParams CreateProcessParams()
		{
			var config = GetEmulationConfiguration(_config);

			if (config == null)
			{
				return null;
			}

			var args = "";

			//Bandwidth
			var bandwidthConfig = config as BandwidthConfiguration;
			if (bandwidthConfig != null)
			{
				args = "Bash/netem-bandwidth.sh -ips=" + FormatEndpointsParamString(bandwidthConfig.TargetEndpoints.Endpoints, ParamType.Hostname) + " " +
					   bandwidthConfig.DownstreamBandwidth + " " + _config.DurationInSeconds + "s";
			}
			//Corruption
			var corruptionConfig = config as CorruptionConfiguration;
			if (corruptionConfig != null)
			{
				var pt = corruptionConfig.PacketPercentage * 100; //e.g., 0.05 * 100 = 5...
				args = "Bash/netem-corrupt.sh -ips=" + FormatEndpointsParamString(corruptionConfig.TargetEndpoints.Endpoints, ParamType.Hostname) + " " +
					   pt + " " + _config.DurationInSeconds + "s";
			}
			/*/Disconnect TODO -CT
			var disconnectConfig = config as DisconnectConfiguration;
			if (disconnectConfig != null)
			{
				args = "Bash/netem-disconnect -ips= " + FormatEndpointsParamString(disconnectConfig.TargetEndpoints.Endpoints, ParamType.Uri) + 
				       " " + _config.DurationInSeconds + "s";
			}
			*/
			//Latency
			var latencyConfig = config as LatencyConfiguration;
			if (latencyConfig != null)
			{
				args = "Bash/netem-latency.sh -ips=" + FormatEndpointsParamString(latencyConfig.TargetEndpoints.Endpoints, ParamType.Hostname) + " " +
						latencyConfig.FixedLatencyDelayMilliseconds + "ms " + " " + _config.DurationInSeconds + "s";
			}
			//Loss
			var lossConfig = config as LossConfiguration;
			if (lossConfig != null)
			{
				double lossRate = lossConfig.LossRate * 100;
				double burstRate = 0;
				if (lossConfig?.BurstRate > 0)
				{
					burstRate = lossConfig.BurstRate * 100;
				}
				args = "Bash/netem-loss.sh -ips=" + FormatEndpointsParamString(lossConfig.TargetEndpoints.Endpoints, ParamType.Hostname) +
					   " " + lossRate + " " + burstRate + " " + _config.DurationInSeconds;
			}
			//Reorder
			var reorderConfig = config as ReorderConfiguration;
			if (reorderConfig != null)
			{
				var correlationpt = reorderConfig.CorrelationPercentage * 100;
				var packetpt = reorderConfig.PacketPercentage * 100;

				args = "Bash/netem-reorder.sh -ips=" + FormatEndpointsParamString(reorderConfig.TargetEndpoints.Endpoints, ParamType.Hostname) +
					   " " + packetpt + " " + " " + correlationpt + " " + _config.DurationInSeconds;
			}

			return new ProcessParams(new System.IO.FileInfo("/usr/bin/bash"), args);
		}

		static string FormatEndpointsParamString(IEnumerable<Endpoint> endpoints, ParamType type)
		{
			string value = "";
			string param = "";
			// TODO: Add support for Port and Protocol... Right now, Protocol = All...
			// If Port type, check to see if all Endpoint Port properties are -1 or less... 
			// If so, then return an empty string...
			var enumerable = endpoints as IList<Endpoint> ?? endpoints.ToList();
			if (type == ParamType.Port && enumerable.All(endpoint => endpoint.Port <= -1))
			{
				return "";
			}

			foreach (var endpoint in enumerable)
			{
				if (type == ParamType.Hostname)
				{
					var endpointHostName = endpoint.Hostname; //user can supply Url or domain in Chaos.config...
					if (endpoint.Hostname.StartsWith("https", StringComparison.Ordinal) || endpoint.Hostname.StartsWith("http", StringComparison.Ordinal))
					{
						var uri = new Uri(endpoint.Hostname);
						endpointHostName = uri.DnsSafeHost;
					}

					var ips = GetIpAddressesForEndpoint(endpointHostName);
					value = ips.Aggregate(value, (current, ip) => current + (IPAddress.Parse(string.Join(".", ip)) + ","));
				}
				else //TODO...
				{
					param = endpoint.Port.ToString();
				}
			}

			return value.TrimEnd(',');
		}

		EmulationConfiguration GetEmulationConfiguration(Config.NetworkEmulation config)
		{
			EmulationConfiguration emulationConfiguration = null;

			if (config.EmulationType == NetworkEmProfile.Bandwidth)
			{
				emulationConfiguration = new BandwidthConfiguration
				{
					DownstreamBandwidth = config.BandwidthDownstreamSpeed,
					UpstreamBandwidth = config.BandwidthUpstreamSpeed,
					TargetEndpoints = config.TargetEndpoints
				};
			}
			if (config.EmulationType == NetworkEmProfile.Corruption)
			{
				emulationConfiguration = new CorruptionConfiguration
				{
					PacketPercentage = config.PacketPercentage,
					TargetEndpoints = config.TargetEndpoints
				};
			}
			if (config.EmulationType == NetworkEmProfile.Latency)
			{
				emulationConfiguration = new LatencyConfiguration
				{
					FixedLatencyDelayMilliseconds = _config.LatencyDelay,
					TargetEndpoints = config.TargetEndpoints
				};
			}
			/* TODO -CT
			if (config.EmulationType == NetworkEmProfile.Disconnect)
			{
				emulationConfiguration = new DisconnectConfiguration
				{
					ConnectionTime = config.ConnectionTime,
					DisconnectionTime = config.DisconnectionTime,
					PeriodicDisconnectionRate = config.PeriodicDisconnectionRate,
					TargetEndpoints = config.TargetEndpoints
				};
			}
			*/
			if (config.EmulationType == NetworkEmProfile.Loss) //Random and Burst are supported...
			{
				emulationConfiguration = new LossConfiguration
				{
					LossRate = config.LossRate,
					LossType = config.LossType,
					TargetEndpoints = config.TargetEndpoints
				};

				if (config.LossType == LossType.Burst)
				{
					((LossConfiguration) emulationConfiguration).BurstRate = config.BurstRate;
				}
			}
			if (config.EmulationType == NetworkEmProfile.Reorder)
			{
				emulationConfiguration = new ReorderConfiguration
				{
					CorrelationPercentage = config.CorrelationPercentage,
					PacketPercentage = config.PacketPercentage
				};
			}

			return emulationConfiguration;
		}

		static IEnumerable<IPAddress> GetIpAddressesForEndpoint(string hostname)
		{
			try
			{
				return Dns.GetHostEntryAsync(hostname).Result.AddressList;
			}
			catch (SocketException)
			{
				return null;
			}
		}

		internal override void Kill()
		{
			if (Process == null || !Process.IsRunning() || Process.HasExited)
			{
				return;
			}
			System.Threading.Thread.Sleep(4000);
			Process?.Kill();
		}
	}

	enum ParamType
	{
		Hostname,
		Port,
		Protocol
	}
}
