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

			//Latency
			var latencyConfig = config as LatencyConfiguration;
			if (latencyConfig != null)
			{
				args = "Bash/netem-latency.sh -ips=" + FormatEndpointsParamString(latencyConfig.TargetEndpoints.Endpoints, ParamType.Uri) + " " + 
						latencyConfig.FixedLatencyDelayMilliseconds + "ms " + " " + _config.DurationInSeconds + "s";
			}

			//Bandwidth
			var bandwidthConfig = config as BandwidthConfiguration;
			if (bandwidthConfig != null)
			{
				args = "Bash/netem-bandwidth.sh -ips=" + FormatEndpointsParamString(bandwidthConfig.TargetEndpoints.Endpoints, ParamType.Uri) + " " +
					   bandwidthConfig.DownstreamBandwidth + " " + _config.DurationInSeconds + "s";
			}

			/*/Disconnect TODO -CT
			var disconnectConfig = config as DisconnectConfiguration;
			if (disconnectConfig != null)
			{
				args = "Bash/netem-disconnect -ips= " + FormatEndpointsParamString(disconnectConfig.TargetEndpoints.Endpoints, ParamType.Uri) + 
				       " " + _config.DurationInSeconds + "s";
			}

			//Loss TODO -CT
			var lossConfig = config as LossConfiguration;
			if (lossConfig != null)
			{
				string loss = "";
				switch (_config.LossType)
				{
					case LossType.Burst:
						loss = " -lossrate " + lossConfig.BurstRate;

break;
					case LossType.Random:
						loss = " -lossrate " + lossConfig.RandomLossRate;
						break;
					case LossType.Periodic:
						loss = " -lossperiod " + lossConfig.PeriodicLossPeriod;
						break;
				}
				args = "Bash/netem-loss.h -ips=" + FormatEndpointsParamString(lossConfig.TargetEndpoints.Endpoints, ParamType.Uri) + 
				       " " +  loss + " " + _config.DurationInSeconds;
			}
			*/

			return new ProcessParams(new System.IO.FileInfo("/usr/bin/bash"), args);
		}

		static string FormatEndpointsParamString(IEnumerable<Endpoint> endpoints, ParamType type)
		{
			string value = "";
			string param = "";
			//if Port type, check to see if all Endpoint Port properties are empty. If so, then return an empty string...
			if (type == ParamType.Port && endpoints.All(endpoint => string.IsNullOrEmpty(endpoint.Port)))
			{
				return "";
			}

			foreach (var endpoint in endpoints)
			{
				if (type == ParamType.Uri)
				{
					 var endpointHostName = endpoint; //user can supply Url or domain in Chaos.config...
					 if (endpoint.StartsWith("https") || endpoint.StartsWith("http"))
                    			 {
						var uri = new Uri(endpoint);
						endpointHostName = uri.DnsSafeHost;
					}
					
					var ips = GetIpAddressesForEndpoint(endpointHostName);
					foreach (var ip in ips)
					{
						value += IPAddress.Parse(string.Join(".", ip)).ToString() + ",";
					}
				}
				else
				{
					param = endpoint.Port;
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
			if (config.EmulationType == NetworkEmProfile.Latency)
			{
				emulationConfiguration = new LatencyConfiguration
				{
					FixedLatencyDelayMilliseconds = _config.LatencyDelay,
					TargetEndpoints = config.TargetEndpoints
				};
			}
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
			if (config.EmulationType == NetworkEmProfile.Loss)
			{
				emulationConfiguration = new LossConfiguration
				{
					BurstRate = config.BurstRate,
					MaximumBurst = config.MaximumBurst,
					MinimumBurst = config.MinimumBurst,
					PeriodicLossPeriod = config.PeriodicLossPeriod,
					RandomLossRate = config.RandomLossRate,
					TargetEndpoints = config.TargetEndpoints,
					ProtocolLayerType = config.ProtocolLayerType,
					NetworkLayerType = config.NetworkLayerType
				};
			}

			return emulationConfiguration;
		}


		static IEnumerable<IPAddress> GetIpAddressesForEndpoint(string hostname)
		{
			try
			{
				return Dns.GetHostEntry(hostname).AddressList;
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
		Port,
		Uri
	}
}
