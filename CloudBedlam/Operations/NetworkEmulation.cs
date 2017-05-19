using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Linq;
using System.Net.Sockets;
using CloudBedlam.Config;
using CloudBedlam.Extensions;

/*
 
links to learn more about tc/netem...

http://www.tecmint.com/linux-network-configuration-and-troubleshooting-commands/
https://www.excentis.com/blog/use-linux-traffic-control-impairment-node-test-environment-part-1
http://manpages.ubuntu.com/manpages/wily/man8/tc-netem.8.html#contenttoc5


EXAMPLES:

Latency and jitter

Using the netem qdisc we can emulate network latency and jitter on all outgoing packets (man page, read more). Some examples:

$ tc qdisc add dev eth0 root netem delay 100ms
<delay packets for 100ms>

$ tc qdisc add dev eth0 root netem delay 100ms 10ms
<delay packets with value from uniform [90ms-110ms] distribution>

$ tc qdisc add dev eth0 root netem delay 100ms 10ms 25%
<delay packets with value from uniform [90ms-110ms] distribution and 25% \
    correlated with value of previous packet>

$ tc qdisc add dev eth0 root netem delay 100ms 10ms distribution normal
<delay packets with value from normal distribution (mean 100ms, jitter 10ms)>

$ tc qdisc add dev eth0 root netem delay 100ms 10ms 25% distribution normal
<delay packets with value from normal distribution (mean 100ms, jitter 10ms) \
    and 25% correlated with value of previous packet>


Packet loss

Using the netem qdisc packet loss can be emulated as well (man page, read more). Some simple examples:

$ tc qdisc add dev eth0 root netem loss 0.1%
<drop packets randomly with probability of 0.1%>

$ tc qdisc add dev eth0 root netem loss 0.3% 25%
<drop packets randomly with probability of 0.3% and 25% correlated with drop \
    decision for previous packet>
But netem can even emulate more complex loss mechanism, such as the Gilbert-Elliot scheme. This scheme defines 2 states Good (or drop Gap) and Bad (or drop Burst). The drop chances of both states and the chances of switching between states are all provided. See section 3 of this paper for more info.

$ tc qdisc add dev eth0 root netem loss gemodel 1% 10% 70% 0.1%
<drop packets using Gilbert-Elliot scheme with probabilities \
    move-to-burstmode (p) of 1%, move-to-gapmode (r) of 10%, \
    drop-in-burstmode (1-h) of 70% and drop-in-gapmode (1-k) of 0.1%>

*/


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

		/*
			We only really care about emulating outgoing/incoming traffic for specific IPs... -CT
			
			Linux =>
			From https://wiki.linuxfoundation.org/networking/netem
			
			Here is a simple example that only controls traffic to one IP address.

			 # tc qdisc add dev eth0 root handle 1: prio
			 # tc qdisc add dev eth0 parent 1:3 handle 30: \
			tbf rate 20kbit buffer 1600 limit  3000
			 # tc qdisc add dev eth0 parent 30:1 handle 31: \
			netem  delay 200ms 10ms distribution normal
			 # tc filter add dev eth0 protocol ip parent 1:0 prio 3 u32 \
			     match ip dst 65.172.181.4/32 flowid 1:3


			How can I use netem on incoming traffic?

			You need to use the Intermediate Functional Block pseudo-device IFB . This network device allows attaching queuing discplines to incoming packets.

			 # modprobe ifb
			 # ip link set dev ifb0 up
			 # tc qdisc add dev eth0 ingress
			 # tc filter add dev eth0 parent ffff: \ 
			   protocol ip u32 match u32 0 0 flowid 1:1 action mirred egress redirect dev ifb0
			 # tc qdisc add dev ifb0 root netem delay 750ms
		*/

		protected override ProcessParams CreateProcessParams()
		{
			var config = GetEmulationConfiguration(_config);

			if (config == null) return null;

			var args = "";
			//Latency
			var latencyConfig = config as LatencyConfiguration;
			if (latencyConfig != null)
			{
				args = "Bash/netem-ip-latency.sh " + latencyConfig.FixedLatencyDelayMilliseconds + " " +
													 FormatEndpointsParamString(latencyConfig.TargetEndpoints.Endpoints, ParamType.Port) + " " +
													 FormatEndpointsParamString(latencyConfig.TargetEndpoints.Endpoints, ParamType.Uri) + " " +
													 _config.DurationInSeconds + "s";
			}
			//Bandwidth TODO: Convert to bash commands (create sh file per emulation type....) -CT
			var bandwidthConfig = config as BandwidthConfiguration;
			if (bandwidthConfig != null)
			{
				args = "-config bandwidth -dsbandwidth " + bandwidthConfig.DownstreamBandwidth + " -usbandwidth " +
					   bandwidthConfig.UpstreamBandwidth + " -url " + FormatEndpointsParamString(bandwidthConfig.TargetEndpoints.Endpoints, ParamType.Uri) + " -duration " +
					   _config.DurationInSeconds;
			}
			//Disconnect TODO: Convert to bash commands (create sh file per emulation type....) -CT
			var disconnectConfig = config as DisconnectConfiguration;
			if (disconnectConfig != null)
			{
				/*
				-config disconnect -connectiontime 5 -disconnectiontime 15 -disconnectionrate 0.8 -url https://www.bing.com -duration 15
				*/
				args = "-config disconnect -connectiontime " + disconnectConfig.ConnectionTime + " -disconnectiontime " + disconnectConfig.DisconnectionTime + " -disconnectionrate " + disconnectConfig.PeriodicDisconnectionRate + " -url " + FormatEndpointsParamString(disconnectConfig.TargetEndpoints.Endpoints, ParamType.Uri) + " -duration " + _config.DurationInSeconds;
			}
			//Loss TODO: Convert to bash commands (create sh file per emulation type....) -CT
			var lossConfig = config as LossConfiguration;
			if (lossConfig != null)
			{
				//-config loss -losstype random -lossrate 0.9 -protocol udp -url https://www.bing.com -duration 15
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
				args = "-config loss -losstype " + lossConfig.LossType + loss + " -protocol " + lossConfig.ProtocolLayerType + " -url " + FormatEndpointsParamString(lossConfig.TargetEndpoints.Endpoints, ParamType.Uri) + " -duration " + _config.DurationInSeconds;
			}

			return new ProcessParams(null, args);
		}

		static string FormatEndpointsParamString(IEnumerable<Endpoint> endpoints, ParamType type)
		{
			string value = "";
			string param = "";

			if (type == ParamType.Port && endpoints.All(endpoint => string.IsNullOrEmpty(endpoint.Port)))
			{
				return "";
			}

			foreach (var endpoint in endpoints)
			{
				if (type == ParamType.Uri)
				{
					param = endpoint.Uri;
				}
				else
				{
					param = endpoint.Port;
				}

				value += param + ",";
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
			if (Process == null || !Process.IsRunning() || Process.HasExited) return;

			System.Threading.Thread.Sleep(5000); //Give the emulator time to stop and uninstall net driver...
			Process?.Kill();
		}
	}

	enum ParamType
	{
		Port,
		Uri
	}
}
